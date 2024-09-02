using Infrastructure.PostgreSQL;
using Infrastructure.SharedKernel.ValueObjects;
using Service.Chat.Domain.Enums;

namespace Service.Chat.Application.Features.FeatureConversation.Responses;

public class ListConversationResponse : PaginationResult<ConversationViewModel>;

public class ConversationViewModel : BaseEntity
{
    public ConversationType Type { get; set; }
    
    public string? Name { get; set; }
    
    public string? Image { get; set; }
    
    public Guid? LastMessageByUserId { get; set; }
    
    public string? LastMessageByUserName { get; set; }

    public string? LastMessageText { get; set; }
    
    public DateTimeOffset? LastMessageAt { get; set; }
}