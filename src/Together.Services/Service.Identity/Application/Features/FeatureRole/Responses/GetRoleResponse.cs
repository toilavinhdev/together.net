using Infrastructure.PostgreSQL;

namespace Service.Identity.Application.Features.FeatureRole.Responses;

public class GetRoleResponse : TimeTrackingEntity
{
    public string Name { get; set; } = default!;
    
    public string? Description { get; set; }
    
    public bool IsDefault { get; set; }
    
    public List<string> Claims { get; set; } = default!;
}