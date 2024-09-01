using FluentValidation;
using Infrastructure.RabbitMQ;
using Infrastructure.RabbitMQ.Events;
using Infrastructure.SharedKernel.Constants;
using Infrastructure.SharedKernel.Enums;
using Infrastructure.SharedKernel.Exceptions;
using Infrastructure.SharedKernel.Extensions;
using Infrastructure.SharedKernel.Mediator;
using Microsoft.EntityFrameworkCore;
using Service.Community.Application.Features.FeatureReply.Responses;
using Service.Community.Domain;
using Service.Community.Domain.Aggregates.ReplyAggregate;
using Service.Community.Domain.Enums;

namespace Service.Community.Application.Features.FeatureReply.Commands;

public sealed class VoteReplyCommand : IBaseRequest<VoteReplyResponse>
{
    public Guid ReplyId { get; set; }
    
    public VoteType Type { get; set; }
    
    public class Validator : AbstractValidator<VoteReplyCommand>
    {
        public Validator()
        {
            RuleFor(x => x.ReplyId).NotEmpty();
            RuleFor(x => x.Type).NotNull();
        }
    }
    
    internal class Handler(IHttpContextAccessor httpContextAccessor,
        CommunityContext context,
        IRabbitMQClient messageBus) 
        : BaseRequestHandler<VoteReplyCommand, VoteReplyResponse>(httpContextAccessor)
    {
        protected override async Task<VoteReplyResponse> HandleAsync(VoteReplyCommand request, CancellationToken ct)
        {
            if (!await context.Replies.AnyAsync(p => p.Id == request.ReplyId, ct))
                throw new TogetherException(ErrorCodes.Reply.ReplyNotFound);

            var postId = await context.Replies
                .Where(r => r.Id == request.ReplyId)
                .Select(r => r.PostId)
                .FirstOrDefaultAsync(ct);
            
            var replyAuthorId = await context.Replies
                .Where(r => r.Id == request.ReplyId)
                .Select(r => r.CreatedById)
                .FirstOrDefaultAsync(ct);

            var vote = await context.ReplyVotes
                .FirstOrDefaultAsync(v => 
                    v.ReplyId == request.ReplyId && 
                    v.CreatedById == UserClaimsPrincipal.Id, ct);

            // Chưa vote -> tạo vote
            if (vote is null)
            {
                vote = request.MapTo<ReplyVote>();
                vote.MarkUserCreated(UserClaimsPrincipal.Id);
                await context.ReplyVotes.AddAsync(vote, ct);
                await context.SaveChangesAsync(ct);
                PublishCreateNotificationEvent(postId, replyAuthorId, request.ReplyId);
                Message = "Voted";
                return new VoteReplyResponse
                {
                    SourceId = request.ReplyId,
                    Value = vote.Type,
                    IsVoted = true
                };
            }
            
            // Vote trùng type đã có -> xóa vote
            if (vote.Type == request.Type)
            {
                context.ReplyVotes.Remove(vote);
                await context.SaveChangesAsync(ct);
                PublishDeleteNotificationEvent(replyAuthorId, request.ReplyId);
                Message = "Unvoted";
                return new VoteReplyResponse
                {
                    SourceId = request.ReplyId,
                    Value = null,
                    IsVoted = false
                };
            }
            
            // Vote khác type đã có -> update vote
            vote.Type = request.Type;
            vote.MarkUserModified(UserClaimsPrincipal.Id);
            context.ReplyVotes.Update(vote);
            await context.SaveChangesAsync(ct);
            PublishCreateNotificationEvent(postId, replyAuthorId, request.ReplyId);
            Message = "Voted";
            return new VoteReplyResponse
            {
                SourceId = request.ReplyId,
                Value = vote.Type,
                IsVoted = true
            };
        }
        
        private void PublishCreateNotificationEvent(Guid postId, Guid replyAuthorId, Guid replyId)
        {
            if (replyAuthorId == UserClaimsPrincipal.Id) return;
            messageBus.Publish(new CreateNotificationEvent
            {
                EventId = Guid.NewGuid(),
                ReceiverId = replyAuthorId,
                SubjectId = UserClaimsPrincipal.Id,
                NotificationType = (int)NotificationType.VoteReply,
                DirectObjectId = replyId,
                PrepositionalObjectId = postId
            });
        }
        
        private void PublishDeleteNotificationEvent(Guid replyAuthorId, Guid replyId)
        {
            if (replyAuthorId == UserClaimsPrincipal.Id) return;
            messageBus.Publish(new DeleteNotificationEvent
            {
                EventId = Guid.NewGuid(),
                SubjectId = UserClaimsPrincipal.Id,
                NotificationType = (int)NotificationType.VoteReply,
                DirectObjectId = replyId
            });
        }
    }
}