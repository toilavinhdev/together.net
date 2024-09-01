using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure.DataModels;
using Infrastructure.PostgreSQL;
using Service.Community.Domain.Aggregates.PostAggregate;
using Service.Community.Domain.Aggregates.TopicAggregate;

namespace Service.Community.Domain.Aggregates.ForumAggregate;

public class Forum : ModifierTrackingEntity, IAggregateRoot
{
    [StringLength(64)]
    public string Name { get; set; } = default!;
    
    [InverseProperty(nameof(Topic.Forum))]
    public List<Topic>? Topics { get; set; }
    
    [InverseProperty(nameof(Post.Forum))]
    public List<Post>? Posts { get; set; }
}