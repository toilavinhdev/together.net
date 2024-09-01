using Infrastructure.PostgreSQL;
using Infrastructure.SharedKernel.Enums;
using Infrastructure.SharedKernel.ValueObjects;

namespace Service.Identity.Application.Features.FeatureUser.Responses;

public class ListUserResponse : PaginationResult<UserViewModel>;

public class UserViewModel : TimeTrackingEntity
{
    public string UserName { get; set; } = default!;
    
    public UserStatus Status { get; set; }
    
    public string Email { get; set; } = default!;
    
    public string? FullName { get; set; }

    public string? Avatar { get; set; }
}