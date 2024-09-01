using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure.DataModels;
using Infrastructure.PostgreSQL;
using Service.Community.Domain.Aggregates.ForumAggregate;
using Service.Community.Domain.Aggregates.PostAggregate;

namespace Service.Community.Domain.Aggregates.TopicAggregate;

public class Topic : ModifierTrackingEntity, IAggregateRoot
{
    public Guid ForumId { get; set; }
    [ForeignKey(nameof(ForumId))]
    public Forum Forum { get; set; } = default!;

    [StringLength(64)]
    public string Name { get; set; } = default!;
    
    [StringLength(64)]
    public string? Description { get; set; }
    
    [InverseProperty(nameof(Post.Topic))]
    public List<Post>? Posts { get; set; }
}