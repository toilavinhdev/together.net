using FluentValidation;
using Grpc.Net.Client;
using Infrastructure.Redis;
using Infrastructure.SharedKernel.BusinessObjects;
using Infrastructure.SharedKernel.Constants;
using Infrastructure.SharedKernel.Exceptions;
using Infrastructure.SharedKernel.Extensions;
using Infrastructure.SharedKernel.Mediator;
using Infrastructure.SharedKernel.Protos.Identity;
using Microsoft.EntityFrameworkCore;
using Service.Community.Application.Features.FeaturePost.Responses;
using Service.Community.Domain;
using Service.Community.Domain.Enums;

namespace Service.Community.Application.Features.FeaturePost.Queries;

public sealed class GetPostQuery(Guid id) : IBaseRequest<GetPostResponse>
{
    private Guid Id { get; set; } = id;
    
    public class Validator : AbstractValidator<GetPostQuery>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
    
    internal class Handler(IHttpContextAccessor httpContextAccessor, 
        CommunityContext context, 
        IRedisService redisService, 
        AppSettings appSettings) 
        : BaseRequestHandler<GetPostQuery, GetPostResponse>(httpContextAccessor)
    {
        private const int UserViewPostDurationInMinutes = 20;
        
        protected override async Task<GetPostResponse> HandleAsync(GetPostQuery request, CancellationToken ct)
        {
            var post = await context.Posts
                .Include(p => p.Forum)
                .Include(p => p.Topic)
                .Include(p => p.Prefix)
                .Select(post => new GetPostResponse
                {
                    Id = post.Id,
                    SubId = post.SubId,
                    ForumId = post.Forum.Id,
                    ForumName = post.Forum.Name,
                    TopicId = post.Topic.Id,
                    TopicName = post.Topic.Name,
                    PrefixId = post.Prefix!.Id,
                    PrefixName = post.Prefix.Name,
                    PrefixForeground = post.Prefix.Foreground,
                    PrefixBackground = post.Prefix.Background,
                    Title = post.Title,
                    Body = post.Body,
                    CreatedAt = post.CreatedAt,
                    CreatedById = post.CreatedById,
                    CreatedByUserName = string.Empty,
                    CreatedByAvatar = null,
                    ReplyCount = post.Replies!.LongCount(),
                    VoteUpCount = post.PostVotes!.LongCount(vote => vote.Type == VoteType.UpVote),
                    VoteDownCount = post.PostVotes!.LongCount(vote => vote.Type == VoteType.DownVote),
                    ViewCount = post.ViewCount,
                    Status = post.Status,
                    Voted = post.PostVotes!.FirstOrDefault(vote => vote.CreatedById == UserClaimsPrincipal.Id)!.Type
                })
                .FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            
            if (post is null || post.Status == PostStatus.Deleted)
                throw new TogetherException(ErrorCodes.Post.PostNotFound);

            // Attach authors info
            var cachedUser = await redisService.StringGetAsync<IdentityPrivilege>(RedisKeys.Identity<IdentityPrivilege>(post.CreatedById));
            if (cachedUser is not null)
            {
                post.CreatedByUserName = cachedUser.UserName;
                post.CreatedByAvatar = cachedUser.Avatar;
            }
            else
            {
                var channelOptions = new GrpcChannelOptions { HttpHandler = new HttpClientHandler { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator } };
                var channel = GrpcChannel.ForAddress(appSettings.GrpcEndpoints.ServiceIdentity, channelOptions);
                var client = new UserGrpcServerService.UserGrpcServerServiceClient(channel);
                var response = await client.GetGeneralUserGrpcAsync(new GetGeneralUserGrpcRequest { Id = post.CreatedById.ToString() }, cancellationToken: ct);
                if (response.Data is not null)
                {
                    post.CreatedByUserName = response.Data.UserName;
                    post.CreatedByAvatar = response.Data.Avatar;
                }
            }
            
            // Attach view count
            var viewCount = await redisService.StringGetAsync(RedisKeys.CommunityPostView(post.Id));
            post.ViewCount = viewCount!.ToLong();
            
            // Increment post view
            var key = RedisKeys.CommunityPostViewer(post.Id, UserClaimsPrincipal.Id);
            if (await redisService.ExistsAsync(key)) return post;
            await redisService.IncrementAsync(RedisKeys.CommunityPostView(post.Id));
            await redisService.StringSetAsync(key, 0, TimeSpan.FromMinutes(UserViewPostDurationInMinutes));

            return post;
        }
    }
}