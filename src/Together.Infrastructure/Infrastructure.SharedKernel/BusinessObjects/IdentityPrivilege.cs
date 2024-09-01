using Infrastructure.SharedKernel.Enums;

namespace Infrastructure.SharedKernel.BusinessObjects;

public class IdentityPrivilege
{
    public Guid Id { get; set; }
    
    public long SubId { get; set; }

    public string UserName { get; set; } = default!;

    public string Email { get; set; } = default!;
    
    public UserStatus Status { get; set; }
    
    public Gender Gender { get; set; }
    
    public bool IsOfficial { get; set; }
    
    public string? FullName { get; set; }
    
    public string? Avatar { get; set; }
    
    public string? Biography { get; set; }

    public List<string> Permissions { get; set; } = default!;
    
    public DateTimeOffset CreatedAt { get; set; }
}