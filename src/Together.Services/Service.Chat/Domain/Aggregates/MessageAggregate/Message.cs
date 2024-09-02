using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure.DataModels;
using Infrastructure.PostgreSQL;
using Service.Chat.Domain.Aggregates.ConversationAggregate;

namespace Service.Chat.Domain.Aggregates.MessageAggregate;

public class Message : ModifierTrackingEntity, IAggregateRoot, ISoftDeleteDataModel<Guid?>
{
    public Guid ConversationId { get; set; }
    [ForeignKey(nameof(ConversationId))]
    public Conversation Conversation { get; set; } = default!;
    
    public Guid? ParentId { get; set; }
    [ForeignKey(nameof(ParentId))]
    public Message? Parent { get; set; }
    
    [StringLength(256)]
    public string Text { get; set; } = default!;
    
    public Guid? DeletedById { get; set; }
    
    public DateTimeOffset? DeletedAt { get; set; }
    
    [InverseProperty(nameof(Parent))]
    public List<Message>? Children { get; set; }
}