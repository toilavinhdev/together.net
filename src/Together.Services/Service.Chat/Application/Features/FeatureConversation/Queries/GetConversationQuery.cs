using System.Linq.Expressions;
using Infrastructure.SharedKernel.Mediator;
using Microsoft.EntityFrameworkCore;
using Service.Chat.Application.Features.FeatureConversation.Expresions;
using Service.Chat.Application.Features.FeatureConversation.Responses;
using Service.Chat.Domain;
using Service.Chat.Domain.Aggregates.ConversationAggregate;
using Service.Chat.Domain.Enums;

namespace Service.Chat.Application.Features.FeatureConversation.Queries;

public sealed class GetConversationQuery : IBaseRequest<ConversationViewModel?>
{
    public Guid? ConversationId { get; set; }
    
    public Guid? PrivateReceiverId { get; set; }

    internal class Handler(IHttpContextAccessor httpContextAccessor, ChatContext context) 
        : BaseRequestHandler<GetConversationQuery, ConversationViewModel?>(httpContextAccessor)
    {
        protected override async Task<ConversationViewModel?> HandleAsync(GetConversationQuery request, CancellationToken ct)
        {
            Expression<Func<Conversation, bool>> whereExpression = c => true;

            if (request.ConversationId is not null)
            {
                whereExpression = whereExpression.And(c => c.Id == request.ConversationId);
            }

            if (request.PrivateReceiverId is not null)
            {
                whereExpression = whereExpression.And(c => 
                    c.Type == ConversationType.Private &&
                    c.ConversationParticipants.Count == 2 &&
                    c.ConversationParticipants.All(cp => 
                        cp.ParticipantId == request.PrivateReceiverId || 
                        cp.ParticipantId == UserClaimsPrincipal.Id));
            }
            
            var conversation = await context.Conversations
                .Include(c => c.ConversationParticipants)
                .Where(whereExpression)
                .Select(ConversationExpressions.ConversationViewModelSelector(UserClaimsPrincipal))
                .FirstOrDefaultAsync(ct);

            return conversation;
        }
    }
}