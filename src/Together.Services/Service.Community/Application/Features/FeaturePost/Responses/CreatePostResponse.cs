using Infrastructure.PostgreSQL;

namespace Service.Community.Application.Features.FeaturePost.Responses;

public class CreatePostResponse : TimeTrackingEntity
{
    public Guid ForumId { get; set; }
    
    public Guid TopicId { get; set; }
    
    public Guid? PrefixId { get; set; }
    
    public string Title { get; set; } = default!;

    public string Body { get; set; } = default!;
}