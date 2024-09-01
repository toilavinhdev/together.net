using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure.DataModels;
using Infrastructure.PostgreSQL;
using Infrastructure.SharedKernel.Enums;
using Microsoft.EntityFrameworkCore;

namespace Service.Identity.Domain.Aggregates.UserAggregate;

[Index(nameof(UserName)), Index(nameof(Email))]
public class User : ModifierTrackingEntity, IAggregateRoot
{
    [StringLength(32)]
    public string UserName { get; set; } = default!;
    
    [StringLength(64)]
    public string Email { get; set; } = default!;
    
    public UserStatus Status { get; set; }
    
    [StringLength(256)]
    public string PasswordHash { get; set; } = default!;
    
    public Gender Gender { get; set; }
    
    public bool IsOfficial { get; set; }
    
    [StringLength(64)]
    public string? FullName { get; set; }
    
    [StringLength(256)]
    public string? Avatar { get; set; }
    
    [StringLength(256)]
    public string? Biography { get; set; }

    [InverseProperty(nameof(UserRole.User))]
    public List<UserRole>? UserRoles { get; set; }
}