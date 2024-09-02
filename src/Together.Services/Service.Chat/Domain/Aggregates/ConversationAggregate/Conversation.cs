using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure.DataModels;
using Infrastructure.PostgreSQL;
using Service.Chat.Domain.Aggregates.MessageAggregate;
using Service.Chat.Domain.Enums;

namespace Service.Chat.Domain.Aggregates.ConversationAggregate;

public class Conversation : ModifierTrackingEntity, IAggregateRoot
{
    [StringLength(64)]
    public string? Name { get; set; }
    
    public ConversationType Type { get; set; }
    
    [InverseProperty(nameof(Message.Conversation))]
    public List<Message>? Messages { get; set; }
    
    [InverseProperty(nameof(ConversationParticipant.Conversation))]
    public List<ConversationParticipant> ConversationParticipants { get; set; } = default!;
}