using Microsoft.EntityFrameworkCore;

namespace Infrastructure.PostgreSQL;

public abstract class PostgresDbContext<TContext>(DbContextOptions<TContext> options, PostgresConfig config)
    : DbContext(options) where TContext : DbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema(config.Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TContext).Assembly);
    }
}