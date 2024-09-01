using Infrastructure.PostgreSQL;

namespace Service.Community.Application.Features.FeatureForum.Responses;

public class CreateForumResponse : TimeTrackingEntity
{
    public string Name { get; set; } = default!;
}