using System.Linq.Expressions;
using FluentValidation;
using Grpc.Net.Client;
using Infrastructure.Redis;
using Infrastructure.SharedKernel.BusinessObjects;
using Infrastructure.SharedKernel.Extensions;
using Infrastructure.SharedKernel.Mediator;
using Infrastructure.SharedKernel.Protos.Identity;
using Infrastructure.SharedKernel.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Service.Community.Application.Features.FeatureReply.Responses;
using Service.Community.Domain;
using Service.Community.Domain.Aggregates.ReplyAggregate;
using Service.Community.Domain.Enums;

namespace Service.Community.Application.Features.FeatureReply.Queries;

public sealed class ReplyQuery : IBaseRequest<ReplyResponse>, IPaginationRequest
{
    public int PageIndex { get; set; }
    
    public int PageSize { get; set; }
    
    public Guid? PostId { get; set; }
    
    public Guid? ParentId { get; set; }
    
    public Guid? FocusChildId { get; set; }
    
    public class Validator : AbstractValidator<ReplyQuery>
    {
        public Validator()
        {
            Include(new PaginationValidator());
        }
    }
    
    internal class Handler(IHttpContextAccessor httpContextAccessor,
        CommunityContext context, 
        IRedisService redisService,
        AppSettings appSettings) 
        : BaseRequestHandler<ReplyQuery, ReplyResponse>(httpContextAccessor)
    {
        protected override async Task<ReplyResponse> HandleAsync(ReplyQuery request, CancellationToken ct)
        {
            ReplyViewModel? parent = null;

            ReplyViewModel? focusChild = null;
            
            Expression<Func<Reply, bool>> whereExpression = r => true;

            if (request.PostId is not null) whereExpression = whereExpression.And(reply => reply.PostId == request.PostId);
            
            whereExpression = whereExpression.And(r => r.ParentId == request.ParentId);

            if (request.ParentId is not null)
            {
                parent = await context.Replies
                    .Where(r => r.Id == request.ParentId)
                    .Select(SelectorExpression(0, 0))
                    .FirstOrDefaultAsync(ct);
            }

            if (request.FocusChildId is not null)
            {
                whereExpression = whereExpression.And(r => 
                    r.ParentId == request.ParentId &&
                    r.Id != request.FocusChildId);
                
                focusChild = await context.Replies
                    .Where(r => r.Id == request.FocusChildId)
                    .Select(SelectorExpression(0, 0))
                    .FirstOrDefaultAsync(ct);
            }

            var queryable = context.Replies
                .Where(whereExpression)
                .OrderByDescending(r => r.CreatedAt)
                .AsQueryable();

            var totalRecord = await queryable.LongCountAsync(ct);

            var children = await queryable
                .Paging(request.PageIndex, request.PageSize)
                .Select(SelectorExpression(0, 0))
                .ToListAsync(ct);

            // Attach user to each reply by redis
            foreach (var reply in children) await AttachUserInfo(reply, ct);
            if (parent is not null) await AttachUserInfo(parent, ct);
            if (focusChild is not null) await AttachUserInfo(focusChild, ct);
            
            return new ReplyResponse
            {
                Pagination = new Pagination(request.PageIndex, request.PageSize, totalRecord),
                Result = children,
                Parent = parent,
                FocusChild = focusChild
            };
        }

        private async Task AttachUserInfo(ReplyViewModel reply, CancellationToken ct)
        {
            var cachedUser = await redisService.StringGetAsync<IdentityPrivilege>(RedisKeys.Identity<IdentityPrivilege>(reply.CreatedById));

            if (cachedUser is not null)
            {
                reply.CreatedByUserName = cachedUser.UserName;
                reply.CreatedByAvatar = cachedUser.Avatar;
            }
            else
            {
                {
                    var channelOptions = new GrpcChannelOptions { HttpHandler = new HttpClientHandler { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator } };
                    var channel = GrpcChannel.ForAddress(appSettings.GrpcEndpoints.ServiceIdentity, channelOptions);
                    var client = new UserGrpcServerService.UserGrpcServerServiceClient(channel);
                    var response = await client.GetGeneralUserGrpcAsync(new GetGeneralUserGrpcRequest { Id = reply.CreatedById.ToString() }, cancellationToken: ct);
                    if (response.Data is not null)
                    {
                        reply.CreatedByUserName = response.Data.UserName;
                        reply.CreatedByAvatar = response.Data.Avatar;
                    }
                }
            }
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