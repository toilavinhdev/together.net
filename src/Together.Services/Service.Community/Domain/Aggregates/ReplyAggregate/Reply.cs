using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure.DataModels;
using Infrastructure.PostgreSQL;
using Service.Community.Domain.Aggregates.PostAggregate;

namespace Service.Community.Domain.Aggregates.ReplyAggregate;

public class Reply : ModifierTrackingEntity, IAggregateRoot
{
    public Guid PostId { get; set; }
    [ForeignKey(nameof(PostId))]
    public Post Post { get; set; } = default!;
    
    public int Level { get; set; }
    
    public Guid? ParentId { get; set; }
    [ForeignKey(nameof(ParentId))]
    public Reply? Parent { get; set; }
    
    [StringLength(int.MaxValue)]
    public string Body { get; set; } = default!;
    
    [InverseProperty(nameof(Parent))]
    public List<Reply>? Children { get; set; }
        
    [InverseProperty(nameof(ReplyVote.Reply))]
    public List<ReplyVote>? ReplyVotes { get; set; }
}