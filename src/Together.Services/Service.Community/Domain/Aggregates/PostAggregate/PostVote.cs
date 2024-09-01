using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure.PostgreSQL;
using Service.Community.Domain.Enums;

namespace Service.Community.Domain.Aggregates.PostAggregate;

public class PostVote : ModifierTrackingEntity
{
    public Guid PostId { get; set; }
    [ForeignKey(nameof(PostId))]
    public Post Post { get; set; } = default!;
    
    public VoteType Type { get; set; }
}