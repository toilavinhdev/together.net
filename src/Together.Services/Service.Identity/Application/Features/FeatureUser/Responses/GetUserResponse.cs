using Infrastructure.PostgreSQL;
using Infrastructure.SharedKernel.Enums;

namespace Service.Identity.Application.Features.FeatureUser.Responses;

public class GetUserResponse : TimeTrackingEntity
{
    public string UserName { get; set; } = default!;
    
    public bool IsOfficial { get; set; }
    
    public Gender Gender { get; set; }

    public string? FullName { get; set; }
    
    public string? Avatar { get; set; }
    
    public string? Biography { get; set; }
    
    public long PostCount { get; set; }
    
    public long ReplyCount { get; set; }
}