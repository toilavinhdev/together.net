using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure.DataModels;
using Infrastructure.PostgreSQL;
using Service.Community.Domain.Aggregates.ForumAggregate;
using Service.Community.Domain.Aggregates.PrefixAggregate;
using Service.Community.Domain.Aggregates.ReplyAggregate;
using Service.Community.Domain.Aggregates.TopicAggregate;

namespace Service.Community.Domain.Aggregates.PostAggregate;

public class Post : ModifierTrackingEntity, IAggregateRoot
{
    public Guid ForumId { get; set; }
    [ForeignKey(nameof(ForumId))]
    public Forum Forum { get; set; } = default!;
    
    public Guid TopicId { get; set; }
    [ForeignKey(nameof(TopicId))]
    public Topic Topic { get; set; } = default!;
    
    public Guid? PrefixId { get; set; }
    [ForeignKey(nameof(PrefixId))]
    public Prefix? Prefix { get; set; }
    
    [StringLength(128)]
    public string Title { get; set; } = default!;

    [StringLength(int.MaxValue)]
    public string Body { get; set; } = default!;
    
    public int ViewCount { get; set; }
    
    [InverseProperty(nameof(Reply.Post))]
    public List<Reply>? Replies { get; set; }
    
    [InverseProperty(nameof(PostVote.Post))]
    public List<PostVote>? PostVotes { get; set; }
}