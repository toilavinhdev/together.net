using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure.PostgreSQL;
using Service.Community.Domain.Enums;

namespace Service.Community.Domain.Aggregates.ReplyAggregate;

public class ReplyVote : ModifierTrackingEntity
{
    public Guid ReplyId { get; set; }
    [ForeignKey(nameof(ReplyId))]
    public Reply Reply { get; set; } = default!;
    
    public VoteType Type { get; set; }
}