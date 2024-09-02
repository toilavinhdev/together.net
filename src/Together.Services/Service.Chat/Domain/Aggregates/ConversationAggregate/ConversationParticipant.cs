using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Service.Chat.Domain.Aggregates.ConversationAggregate;

[PrimaryKey(nameof(ConversationId), nameof(ParticipantId))]
public class ConversationParticipant
{
    public Guid ConversationId { get; set; }
    [ForeignKey(nameof(ConversationId))]
    public Conversation Conversation { get; set; } = default!;

    public Guid ParticipantId { get; set; }
}