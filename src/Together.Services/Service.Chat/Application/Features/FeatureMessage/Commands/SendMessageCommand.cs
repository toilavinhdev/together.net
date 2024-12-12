using FluentValidation;
using Infrastructure.Redis;
using Infrastructure.Redis.Events;
using Infrastructure.SharedKernel.BusinessObjects;
using Infrastructure.SharedKernel.Constants;
using Infrastructure.SharedKernel.Exceptions;
using Infrastructure.SharedKernel.Extensions;
using Infrastructure.SharedKernel.Mediator;
using Microsoft.EntityFrameworkCore;
using Service.Chat.Application.Features.FeatureMessage.Responses;
using Service.Chat.Domain;
using Service.Chat.Domain.Aggregates.MessageAggregate;

namespace Service.Chat.Application.Features.FeatureMessage.Commands;

public sealed class SendMessageCommand : IBaseRequest<SendMessageResponse>
{
    public Guid ConversationId { get; set; }

    public string Text { get; set; } = default!;
    
    public class Validator : AbstractValidator<SendMessageCommand>
    {
        public Validator()
        {
            RuleFor(x => x.ConversationId).NotEmpty();
            RuleFor(x => x.Text).NotEmpty();
        }
    }

    internal class Handler(IHttpContextAccessor httpContextAccessor, 
        ChatContext context, 
        IRedisService redisService) 
        : BaseRequestHandler<SendMessageCommand, SendMessageResponse>(httpContextAccessor)
    {
        protected override async Task<SendMessageResponse> HandleAsync(SendMessageCommand request, CancellationToken ct)
        {
            if (!await context.Conversations.AnyAsync(c => c.Id == request.ConversationId, ct))
                throw new TogetherException(ErrorCodes.Conversation.ConversationNotFound);

            if (!await context.ConversationParticipants.AnyAsync(cp =>
                    cp.ConversationId == request.ConversationId &&
                    cp.ParticipantId == UserClaimsPrincipal.Id, ct))
                throw new TogetherException(ErrorCodes.Conversation.HaveNotJoinedConversation);

            var message = request.MapTo<Message>();
            message.MarkUserCreated(UserClaimsPrincipal.Id);

            await context.Messages.AddAsync(message, ct);

            await context.SaveChangesAsync(ct);

            await PublishSendMessageSocketEvent(message, ct);
            
            return message.MapTo<SendMessageResponse>();
        }

        private async Task PublishSendMessageSocketEvent(Message message, CancellationToken ct)
        {
            var participantIds = await context.ConversationParticipants
                .Where(cp => cp.ConversationId == message.ConversationId && cp.ParticipantId != UserClaimsPrincipal.Id)
                .Select(cp => cp.ParticipantId.ToString())
                .ToListAsync(ct);
            
            var currentUser = await redisService.StringGetAsync<IdentityPrivilege>(
                RedisKeys.Identity<IdentityPrivilege>(UserClaimsPrincipal.Id));
            if (currentUser is null) return;
            
            await redisService.PublishAsync(new SendMessageSocketEvent
            {
                CorrelationId = CorrelationId(),
                SocketIds = [..participantIds],
                ConversationId = message.ConversationId,
                Text = message.Text,
                CreatedAt = message.CreatedAt,
                CreatedById = currentUser.Id,
                CreatedByUserName = currentUser.UserName,
                CreatedByAvatar = currentUser.Avatar,
            });
        }
    }
}