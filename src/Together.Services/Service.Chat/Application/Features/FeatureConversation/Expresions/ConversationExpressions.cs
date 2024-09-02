using System.Linq.Expressions;
using Infrastructure.SharedKernel.ValueObjects;
using Service.Chat.Application.Features.FeatureConversation.Responses;
using Service.Chat.Domain.Aggregates.ConversationAggregate;
using Service.Chat.Domain.Enums;

namespace Service.Chat.Application.Features.FeatureConversation.Expresions;

public static class ConversationExpressions
{
    public static Expression<Func<Conversation, ConversationViewModel>> ConversationViewModelSelector(UserClaimsPrincipal currentUserClaims)
    {
        return c => new ConversationViewModel
        {
            Id = c.Id,
            SubId = c.SubId,
            Type = c.Type,
            Name = c.Type == ConversationType.Group
                ? c.Name
                : c.ConversationParticipants
                    .FirstOrDefault(cp => cp.ParticipantId != currentUserClaims.Id)!
                    .ParticipantId.ToString(), // replace with other Partner UserName
            Image = c.Type == ConversationType.Group
                ? null
                : string.Empty, // replace with other Partner Avatar
            LastMessageByUserId = c.Messages!
                .OrderByDescending(m => m.CreatedAt)
                .FirstOrDefault()!
                .CreatedById,
            LastMessageByUserName = c.Messages!
                .OrderByDescending(m => m.CreatedAt)
                .FirstOrDefault()!
                .CreatedById.ToString(), // replace with other CreatedBy.UserName
            LastMessageText = c.Messages!
                .OrderByDescending(m => m.CreatedAt)
                .FirstOrDefault()!
                .Text,
            LastMessageAt = c.Messages!
                .OrderByDescending(m => m.CreatedAt)
                .FirstOrDefault()!
                .CreatedAt
        };
    }
}