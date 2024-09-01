using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure.DataModels;
using Infrastructure.PostgreSQL;
using Service.Community.Domain.Aggregates.PostAggregate;

namespace Service.Community.Domain.Aggregates.PrefixAggregate;

public class Prefix : ModifierTrackingEntity, IAggregateRoot
{
    public string Name { get; set; } = default!;

    public string Foreground { get; set; } = default!;

    public string Background { get; set; } = default!;
    
    [InverseProperty(nameof(Post.Prefix))]
    public List<Post>? Posts { get; set; }
}