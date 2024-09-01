using Infrastructure.PostgreSQL;

namespace Service.Community.Application.Features.FeatureForum.Responses;

public class GetForumResponse : TimeTrackingEntity
{
    public string Name { get; set; } = default!;
}