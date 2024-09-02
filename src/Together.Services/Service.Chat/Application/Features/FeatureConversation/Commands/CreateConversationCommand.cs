using FluentValidation;
using Infrastructure.Redis;
using Infrastructure.SharedKernel.BusinessObjects;
using Infrastructure.SharedKernel.Constants;
using Infrastructure.SharedKernel.Exceptions;
using Infrastructure.SharedKernel.Mediator;
using Microsoft.EntityFrameworkCore;
using Service.Chat.Domain;
using Service.Chat.Domain.Aggregates.ConversationAggregate;
using Service.Chat.Domain.Enums;

namespace Service.Chat.Application.Features.FeatureConversation.Commands;

public sealed class CreateConversationCommand : IBaseRequest<Guid>
{
    public List<Guid> OtherParticipantIds { get; set; } = default!;
    
    public ConversationType Type { get; set; }
    
    public string? Name { get; set; }
    
    public class Validator : AbstractValidator<CreateConversationCommand>
    {
        public Validator()
        {
            RuleFor(x => x.OtherParticipantIds).NotNull().Must(x => x.Count > 0);
            When(x => x.Type == ConversationType.Private, () =>
            {
                RuleFor(x => x.OtherParticipantIds.Count == 1);
            });
        }
    }
    
    internal class Handler(IHttpContextAccessor httpContextAccessor, ChatContext context, IRedisService redisService)
        : BaseRequestHandler<CreateConversationCommand, Guid>(httpContextAccessor)
    {
        protected override async Task<Guid> HandleAsync(CreateConversationCommand request, CancellationToken ct)
        {
            // validate users
            foreach (var participantId in request.OtherParticipantIds)
            {
                var user = await redisService.StringGetAsync<IdentityPrivilege>(
                    RedisKeys.Identity<IdentityPrivilege>(participantId));
                if (user is null) throw new TogetherException(ErrorCodes.User.UserNotFound, participantId.ToString());
            }

            if (request.Type == ConversationType.Private)
            {
                var isExist = await context.Conversations
                    .Include(c => c.ConversationParticipants)
                    .AnyAsync(c => 
                        c.ConversationParticipants.Count == 2 &&
                        c.ConversationParticipants.All(cp => 
                                cp.ParticipantId == request.OtherParticipantIds.First() ||  
                                cp.ParticipantId == UserClaimsPrincipal.Id), 
                        ct);
                if (isExist) throw new TogetherException(ErrorCodes.Conversation.PrivateConversationAlreadyExists);
            }

            var conversation = new Conversation
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                ConversationParticipants = request.OtherParticipantIds
                    .Union([UserClaimsPrincipal.Id])
                    .Select(userId => new ConversationParticipant
                    {
                        ParticipantId = userId
                    })
                    .ToList()
            };
            conversation.MarkUserCreated(UserClaimsPrincipal.Id);

            await context.Conversations.AddAsync(conversation, ct);

            await context.SaveChangesAsync(ct);

            return conversation.Id;
        }
    }
}