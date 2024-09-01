using System.Linq.Expressions;
using FluentValidation;
using Infrastructure.Redis;
using Infrastructure.SharedKernel.BusinessObjects;
using Infrastructure.SharedKernel.Extensions;
using Infrastructure.SharedKernel.Mediator;
using Infrastructure.SharedKernel.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Service.Community.Application.Features.FeatureReply.Responses;
using Service.Community.Domain;
using Service.Community.Domain.Aggregates.ReplyAggregate;
using Service.Community.Domain.Enums;

namespace Service.Community.Application.Features.FeatureReply.Queries;

public sealed class ListReplyQuery : IBaseRequest<ListReplyResponse>, IPaginationRequest
{
    public int PageIndex { get; set; }
    
    public int PageSize { get; set; }
    
    public Guid? PostId { get; set; }
    
    public Guid? ParentId { get; set; }
    
    public class Validator : AbstractValidator<ListReplyQuery>
    {
        public Validator()
        {
            Include(new PaginationValidator());
        }
    }
    
    internal class Handler(IHttpContextAccessor httpContextAccessor, CommunityContext context, IRedisService redisService) 
        : BaseRequestHandler<ListReplyQuery, ListReplyResponse>(httpContextAccessor)
    {
        protected override async Task<ListReplyResponse> HandleAsync(ListReplyQuery request, CancellationToken ct)
        {
            Expression<Func<Reply, bool>> whereExpression = r => true;

            if (request.PostId is not null) whereExpression = whereExpression.And(reply => reply.PostId == request.PostId);

            whereExpression = whereExpression.And(r => r.ParentId == request.ParentId);
            
            var queryable = context.Replies
                .Where(whereExpression)
                .OrderByDescending(r => r.CreatedAt)
                .AsQueryable();

            var totalRecord = await queryable.LongCountAsync(ct);

            var replies = await queryable
                .Paging(request.PageIndex, request.PageSize)
                .Select(SelectorExpression(0, 0))
                .ToListAsync(ct);

            // Attach user to each reply by redis
            var cachedUsers = new List<IdentityPrivilege>();
            foreach (var reply in replies)
            {
                var cachedUser = cachedUsers.FirstOrDefault(x => x.Id == reply.CreatedById) 
                           ?? await redisService.StringGetAsync<IdentityPrivilege>(RedisKeys.Identity<IdentityPrivilege>(reply.CreatedById));
                if (cachedUser is not null && cachedUsers.All(x => x.Id != reply.CreatedById))
                    cachedUsers.Add(cachedUser);
                reply.CreatedByUserName = cachedUser?.UserName ?? reply.CreatedByUserName;
                reply.CreatedByAvatar = cachedUser?.Avatar;
            }
            
            return new ListReplyResponse
            {
                Pagination = new Pagination(request.PageIndex, request.PageSize, totalRecord),
                Result = replies
            };
        }
        
        private Expression<Func<Reply, ReplyViewModel>> SelectorExpression(int maxDepth, int childCount)
        {
            maxDepth--;
        
            Expression<Func<Reply, ReplyViewModel>> selectorExpression = r => new ReplyViewModel
            {
                Id = r.Id,
                SubId = r.SubId,
                PostId = r.PostId,
                ParentId = r.ParentId,
                Level = r.Level,
                Body = r.Body,
                CreatedById = r.CreatedById,
                CreatedByUserName = string.Empty,
                CreatedByAvatar = null,
                VoteUpCount = r.ReplyVotes!.LongCount(vote => vote.Type == VoteType.UpVote),
                VoteDownCount = r.ReplyVotes!.LongCount(vote => vote.Type == VoteType.DownVote),
                Voted = r.ReplyVotes!.FirstOrDefault(vote => vote.CreatedById == UserClaimsPrincipal.Id)!.Type,
                ChildCount = r.Children!.LongCount(),
                CreatedAt = r.CreatedAt,
                Children = maxDepth == -1
                    ? null
                    : r.Children!
                        .AsQueryable()
                        .OrderByDescending(child => child.CreatedAt)
                        .Take(childCount)
                        .Select(SelectorExpression(maxDepth, childCount))
                        .ToList()
            };

            return selectorExpression;
        }
    }
}