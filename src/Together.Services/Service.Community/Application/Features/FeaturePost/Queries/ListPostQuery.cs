using System.Linq.Expressions;
using FluentValidation;
using Grpc.Net.Client;
using Infrastructure.Redis;
using Infrastructure.SharedKernel.BusinessObjects;
using Infrastructure.SharedKernel.Constants;
using Infrastructure.SharedKernel.Exceptions;
using Infrastructure.SharedKernel.Extensions;
using Infrastructure.SharedKernel.Mediator;
using Infrastructure.SharedKernel.Protos.Identity;
using Infrastructure.SharedKernel.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Service.Community.Application.Features.FeaturePost.Responses;
using Service.Community.Domain;
using Service.Community.Domain.Aggregates.PostAggregate;
using Service.Community.Domain.Enums;

namespace Service.Community.Application.Features.FeaturePost.Queries;

public sealed class ListPostQuery : IBaseRequest<ListPostResponse>, IPaginationRequest
{
    public int PageIndex { get; set; }
    
    public int PageSize { get; set; }
    
    public Guid? TopicId { get; set; }
    
    public Guid? UserId { get; set; }
    
    public string? Search { get; set; }

    public string? Sort { get; set; }
    
    public class Validator : AbstractValidator<ListPostQuery>
    {
        public Validator()
        {
            Include(new PaginationValidator());
        }
    }
    
    internal class Handler(IHttpContextAccessor httpContextAccessor, 
        CommunityContext context, 
        AppSettings appSettings,
        IRedisService redisService) 
        : BaseRequestHandler<ListPostQuery, ListPostResponse>(httpContextAccessor)
    {
        protected override async Task<ListPostResponse> HandleAsync(ListPostQuery request, CancellationToken ct)
        {
            var extra = new Dictionary<string, object>();

            // Build where expression
            Expression<Func<PostViewModel, bool>> whereExpression = x => true;
            whereExpression = whereExpression.And(x => x.Status != PostStatus.Deleted);
            if (request.TopicId is not null)
            {
                var topic = await context.Topics
                    .Include(t => t.Forum)
                    .FirstOrDefaultAsync(x => x.Id == request.TopicId, ct);
                if (topic is null) throw new TogetherException(ErrorCodes.Topic.TopicNotFound);
                whereExpression = whereExpression.And(x => x.TopicId == request.TopicId);
                extra.Add("topicId", topic.Id.ToString());
                extra.Add("topicName", topic.Name);
                extra.Add("forumId", topic.Forum.Id.ToString());
                extra.Add("forumName", topic.Forum.Name);
            }
            if (request.UserId is not null)
            {
                whereExpression = whereExpression.And(x => x.Author.Id == request.UserId);
            }
            if (!string.IsNullOrEmpty(request.Search))
            {
                whereExpression = whereExpression.And(p =>
                    p.Title.ToLower().Contains(request.Search.ToLower()));
            }

            //Build sort expression
            Expression<Func<PostViewModel, object>> sortExpression = x => x.CreatedAt;
            request.Sort ??= "-CreatedAt";
            if (request.Sort.Contains("VoteUpCount")) sortExpression = x => x.VoteUpCount;
            if (request.Sort.Contains("VoteDownCount")) sortExpression = x => x.VoteDownCount;

            var query = context.Posts
                .Include(p => p.Topic)
                .Include(p => p.Prefix)
                .Include(p => p.Replies)
                .Include(p => p.PostVotes)
                .Select(p => new PostViewModel
                {
                    Id = p.Id,
                    SubId = p.SubId,
                    ForumId = p.ForumId,
                    TopicId = p.Topic.Id,
                    TopicName = p.Topic.Name,
                    PrefixId = p.Prefix!.Id,
                    PrefixName = p.Prefix!.Name,
                    PrefixBackground = p.Prefix!.Background,
                    PrefixForeground = p.Prefix!.Foreground,
                    Title = p.Title,
                    Body = p.Body,
                    Author = new GeneralUser
                    {
                        Id = p.CreatedById,
                        IsOfficial = false,
                        UserName = string.Empty,
                        Avatar = null
                    },
                    CreatedAt = p.CreatedAt,
                    Status = p.Status,
                    ReplyCount = p.Replies!.LongCount(),
                    VoteUpCount = p.PostVotes!.LongCount(x => x.Type == VoteType.UpVote),
                    VoteDownCount = p.PostVotes!.LongCount(x => x.Type == VoteType.DownVote)
                })
                .Where(whereExpression)
                .Sort(sortExpression, request.Sort.StartsWith('+'))
                .Paging(request.PageIndex, request.PageSize);

            var posts = await query.ToListAsync(ct);

            // Attach user info
            foreach (var post in posts)
            {
                var cachedUser = await redisService.StringGetAsync<IdentityPrivilege>(RedisKeys.Identity<IdentityPrivilege>(post.Author.Id));

                if (cachedUser is not null)
                {
                    post.Author.UserName = cachedUser.UserName;
                    post.Author.Avatar = cachedUser.Avatar;
                }
                else
                {
                    var channelOptions = new GrpcChannelOptions { HttpHandler = new HttpClientHandler { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator } };
                    var channel = GrpcChannel.ForAddress(appSettings.GrpcEndpoints.ServiceIdentity, channelOptions);
                    var client = new UserGrpcServerService.UserGrpcServerServiceClient(channel);
                    var response = await client.GetGeneralUserGrpcAsync(new GetGeneralUserGrpcRequest{Id = post.Author.Id.ToString()}, cancellationToken: ct);
                    post.Author.UserName = response.Data.UserName;
                    post.Author.Avatar = response.Data.Avatar;
                }
            }

            var totalRecord = await query.LongCountAsync(ct);
            return new ListPostResponse
            {
                Pagination = new Pagination(request.PageIndex, request.PageSize, totalRecord),
                Result = posts,
                Extra = extra
            };
        }
    }
}