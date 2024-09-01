using Infrastructure.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Service.Identity.Domain.Aggregates.RoleAggregate;
using Service.Identity.Domain.Aggregates.UserAggregate;

namespace Service.Identity.Domain;

public sealed class IdentityContext(DbContextOptions<IdentityContext> options, PostgresConfig config) 
    : PostgresDbContext<IdentityContext>(options, config)
{
    public DbSet<User> Users { get; init; } = default!;
    
    public DbSet<Role> Roles { get; init; } = default!;
    
    public DbSet<UserRole> UserRoles { get; init; } = default!;
}