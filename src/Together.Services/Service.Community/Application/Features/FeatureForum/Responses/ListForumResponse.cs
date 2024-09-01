using Infrastructure.PostgreSQL;

namespace Service.Community.Application.Features.FeatureForum.Responses;

public class ForumViewModel : TimeTrackingEntity
{
    public string Name { get; set; } = default!;
    
    public List<TopicViewModel>? Topics { get; set; }
}

public class TopicViewModel : TimeTrackingEntity
{
    public Guid ForumId { get; set; }

    public string ForumName { get; set; } = default!;
    
    public string Name { get; set; } = default!;
    
    public string? Description { get; set; }
    
    public long PostCount { get; set; }
    
    public long ReplyCount { get; set; }
}