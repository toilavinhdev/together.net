using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Service.Identity.Domain.Aggregates.RoleAggregate;

namespace Service.Identity.Domain.Aggregates.UserAggregate;

[PrimaryKey(nameof(UserId), nameof(RoleId))]
public class UserRole
{
    public Guid UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = default!;
    
    public Guid RoleId { get; set; }
    [ForeignKey(nameof(RoleId))]
    public Role Role { get; set; } = default!;
}