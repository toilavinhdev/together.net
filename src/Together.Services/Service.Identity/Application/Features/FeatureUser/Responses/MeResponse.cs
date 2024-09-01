using Infrastructure.PostgreSQL;
using Infrastructure.SharedKernel.Enums;

namespace Service.Identity.Application.Features.FeatureUser.Responses;

public class MeResponse : BaseEntity
{
    public string UserName { get; set; } = default!;

    public string Email { get; set; } = default!;
    
    public UserStatus Status { get; set; }
    
    public string? Avatar { get; set; }
}