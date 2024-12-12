using FluentValidation;
using Infrastructure.RabbitMQ;
using Infrastructure.RabbitMQ.Events;
using Infrastructure.SharedKernel.Constants;
using Infrastructure.SharedKernel.Enums;
using Infrastructure.SharedKernel.Exceptions;
using Infrastructure.SharedKernel.Extensions;
using Infrastructure.SharedKernel.Mediator;
using Microsoft.EntityFrameworkCore;
using Service.Community.Application.Features.FeaturePost.Responses;
using Service.Community.Domain;
using Service.Community.Domain.Aggregates.PostAggregate;
using Service.Community.Domain.Enums;

namespace Service.Community.Application.Features.FeaturePost.Commands;

public sealed class VotePostCommand : IBaseRequest<VotePostResponse>
{
    public Guid PostId { get; set; }
    
    public VoteType Type { get; set; }
    
    public class Validator : AbstractValidator<VotePostCommand>
    {
        public Validator()
        {
            RuleFor(x => x.PostId).NotEmpty();
            RuleFor(x => x.Type).NotNull();
        }
    }
    
    internal class Handler(IHttpContextAccessor httpContextAccessor,
        CommunityContext context,
        IRabbitMQClient messageBus) 
        : BaseRequestHandler<VotePostCommand, VotePostResponse>(httpContextAccessor)
    {
        protected override async Task<VotePostResponse> HandleAsync(VotePostCommand request, CancellationToken ct)
        {
            if (!await context.Posts.AnyAsync(p => p.Id == request.PostId, ct))
                throw new TogetherException(ErrorCodes.Post.PostNotFound);
            
            var postAuthorId = await context.Posts
                .Where(p => p.Id == request.PostId)
                .Select(p => p.CreatedById)
                .FirstOrDefaultAsync(ct);

            var vote = await context.PostVotes
                .FirstOrDefaultAsync(v =>
                    v.PostId == request.PostId && 
                    v.CreatedById == UserClaimsPrincipal.Id, ct);
            
            // Chưa vote -> tạo vote
            if (vote is null)
            {
                vote = request.MapTo<PostVote>();
                vote.MarkUserCreated(UserClaimsPrincipal.Id);
                await context.PostVotes.AddAsync(vote, ct);
                await context.SaveChangesAsync(ct);
                PublishCreateNotificationEvent(postAuthorId, request.PostId);
                Message = "Voted";
                return new VotePostResponse
                {
                    SourceId = request.PostId,
                    Value = vote.Type,
                    IsVoted = true
                };
            }
            
            // Vote trùng type đã có -> xóa vote -> xóa notification
            if (vote.Type == request.Type)
            {
                context.PostVotes.Remove(vote);
                await context.SaveChangesAsync(ct);
                PublishDeleteNotificationEvent(postAuthorId, request.PostId);
                Message = "Unvoted";
                return new VotePostResponse
                {
                    SourceId = request.PostId,
                    Value = null,
                    IsVoted = false
                };
            }
            
            // Vote khác type đã có -> update vote
            vote.Type = request.Type;
            vote.MarkUserModified(UserClaimsPrincipal.Id);
            context.PostVotes.Update(vote);
            await context.SaveChangesAsync(ct);
            PublishCreateNotificationEvent(postAuthorId, request.PostId);
            Message = "Voted";
            return new VotePostResponse
            {
                SourceId = request.PostId,
                Value = vote.Type,
                IsVoted = true
            };
        }

        private void PublishCreateNotificationEvent(Guid postAuthorId, Guid postId)
        {
            if (postAuthorId == UserClaimsPrincipal.Id) return;
            messageBus.Publish(new CreateNotificationEvent
            {
                EventId = Guid.NewGuid(),
                CorrelationId = CorrelationId(),
                ReceiverId = postAuthorId,
                SubjectId = UserClaimsPrincipal.Id,
                NotificationType = (int)NotificationType.VotePost,
                DirectObjectId = postId
            });
        }
        
        private void PublishDeleteNotificationEvent(Guid postAuthorId, Guid postId)
        {
            if (postAuthorId == UserClaimsPrincipal.Id) return;
            messageBus.Publish(new DeleteNotificationEvent
            {
                EventId = Guid.NewGuid(),
                CorrelationId = CorrelationId(),
                SubjectId = UserClaimsPrincipal.Id,
                NotificationType = (int)NotificationType.VotePost,
                DirectObjectId = postId
            });
        }
    }
}