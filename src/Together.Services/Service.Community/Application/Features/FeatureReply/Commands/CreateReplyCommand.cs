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

namespace Service.Community.Application.Features.FeatureReply.Commands;

public sealed class CreateReplyCommand : IBaseRequest<CreateReplyResponse>
{
    public Guid PostId { get; set; }
    
    public Guid? ParentId { get; set; }
    
    public string Body { get; set; } = default!;
    
    public class Validator : AbstractValidator<CreateReplyCommand>
    {
        public Validator()
        {
            RuleFor(x => x.PostId).NotEmpty();
            RuleFor(x => x.Body).NotEmpty();
        }
    }
    
    internal class Handler(IHttpContextAccessor httpContextAccessor,
        CommunityContext context, 
        IRabbitMQClient messageBus) 
        : BaseRequestHandler<CreateReplyCommand, CreateReplyResponse>(httpContextAccessor)
    {
        protected override async Task<CreateReplyResponse> HandleAsync(CreateReplyCommand request, CancellationToken ct)
        {
            if (!await context.Posts.AnyAsync(p => p.Id == request.PostId, ct))
                throw new TogetherException(ErrorCodes.Post.PostNotFound);

            var postAuthorId = await context.Posts
                .Where(p => p.Id == request.PostId)
                .Select(p => p.CreatedById)
                .FirstOrDefaultAsync(ct);

            Reply? parent = null;

            if (request.ParentId is not null)
            {
                parent = await context.Replies.FirstOrDefaultAsync(
                    r => r.Id == request.ParentId && r.PostId == request.PostId, ct);
                if (parent is null) throw new TogetherException(ErrorCodes.Reply.ReplyNotFound);
            }

            var reply = request.MapTo<Reply>();
            reply.Level = parent is null ? 0 : parent.Level + 1;
            reply.MarkUserCreated(UserClaimsPrincipal.Id);

            await context.Replies.AddAsync(reply, ct);

            await context.SaveChangesAsync(ct);
            
            PublishCreateNotificationEvent(postAuthorId, request.PostId, reply.Id, reply.ParentId);

            return reply.MapTo<CreateReplyResponse>();
        }
        
        private void PublishCreateNotificationEvent(Guid postAuthorId, Guid postId, Guid replyId, Guid? parentId)
        {
            if (postAuthorId == UserClaimsPrincipal.Id) return;

            if (parentId is null)
            {
                messageBus.Publish(new CreateNotificationEvent
                {
                    EventId = Guid.NewGuid(),
                    CorrelationId = CorrelationId(),
                    ReceiverId = postAuthorId,
                    SubjectId = UserClaimsPrincipal.Id,
                    NotificationType = (int)NotificationType.ReplyPost,
                    DirectObjectId = replyId,
                    PrepositionalObjectId = postId
                });
            }
            else
            {
                messageBus.Publish(new CreateNotificationEvent
                {
                    EventId = Guid.NewGuid(),
                    CorrelationId = CorrelationId(),
                    ReceiverId = postAuthorId,
                    SubjectId = UserClaimsPrincipal.Id,
                    NotificationType = (int)NotificationType.ReplyReply,
                    DirectObjectId = replyId,
                    IndirectObjectId = parentId,
                    PrepositionalObjectId = postId,
                });
            }
        }
    }
}