using Infrastructure.PostgreSQL;

namespace Service.Community.Application.Features.FeatureTopic.Responses;

public class GetTopicResponse : TimeTrackingEntity
{
    public Guid ForumId { get; set; }

    public string Name { get; set; } = default!;
    
    public string? Description { get; set; }
}