using Infrastructure.PostgreSQL;
using Infrastructure.SharedKernel.ValueObjects;

namespace Service.Identity.Application.Features.FeatureRole.Responses;

public class ListRoleResponse : PaginationResult<RoleViewModel>;

public class RoleViewModel : TimeTrackingEntity
{
    public string Name { get; set; } = default!;
    
    public string? Description { get; set; }
    
    public bool IsDefault { get; set; }
}