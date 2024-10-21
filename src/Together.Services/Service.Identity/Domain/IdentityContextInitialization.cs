using Infrastructure.SharedKernel.Constants;
using Infrastructure.SharedKernel.Enums;
using Infrastructure.SharedKernel.Extensions;
using Microsoft.EntityFrameworkCore;
using Service.Identity.Domain.Aggregates.RoleAggregate;
using Service.Identity.Domain.Aggregates.UserAggregate;

namespace Service.Identity.Domain;

public static class IdentityContextInitialization
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IdentityContext>();
        var logger = serviceProvider.GetRequiredService<ILogger<IdentityContext>>();
        
        logger.LogInformation("Starting to migrate database");
        await context.Database.MigrateAsync();
        await context.Database.EnsureCreatedAsync();
        var admin = new User
        {
            Id = Guid.NewGuid(),
            FullName = "Administrator",
            UserName = "administrator",
            Email = "admin@together.net",
            IsOfficial = true,
            PasswordHash = "admin".ToSha256(),
            Status = UserStatus.Active,
            Gender = Gender.Other
        };
        if (!await context.Users.AnyAsync(u => u.Email == admin.Email))
        {
            admin.MarkCreated();
            await context.Users.AddAsync(admin);
        }
        var defaultRoles = new List<Role>();
        foreach (var role in Roles)
        {
            var existed = await context.Roles.FirstOrDefaultAsync(r => r.Name == role.Name);
            if (existed is not null) continue;
            role.UserRoles = role.Name == "Admin"
                ? [ new UserRole { UserId = admin.Id }]
                : null;
            role.MarkUserCreated(admin.Id);
            defaultRoles.Add(role);
        }
        
        await context.Roles.AddRangeAsync(defaultRoles);
        
        await context.SaveChangesAsync();
        
        logger.LogInformation("Database migration completed");
    }
    
    private static readonly List<Role> Roles =
    [
        new()
        {
            Id = Guid.NewGuid(),
            Name = "Admin",
            IsDefault = true,
            Description = "Vai trò mặc định có quyền hạn cao nhất",
            Claims = [Policies.All]
        },
        new()
        {
            Id = Guid.NewGuid(),
            Name = "Member",
            IsDefault = true,
            Description = "Vai trò mặc định cho mọi thành viên",
            Claims = Policies.RequiredPolicies().Distinct().ToList()
        }
    ];
}