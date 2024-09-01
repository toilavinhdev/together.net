using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure.DataModels;
using Infrastructure.PostgreSQL;
using Service.Identity.Domain.Aggregates.UserAggregate;

namespace Service.Identity.Domain.Aggregates.RoleAggregate;

public class Role : ModifierTrackingEntity, IAggregateRoot
{
    [StringLength(64)]
    public string Name { get; set; } = default!;

    public bool IsDefault { get; set; }
    
    [StringLength(256)]
    public string? Description { get; set; }

    public List<string> Claims { get; set; } = default!;
    
    [InverseProperty(nameof(UserRole.Role))]
    public List<UserRole>? UserRoles { get; set; }
}