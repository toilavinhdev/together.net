using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.PostgreSQL;

public static class PostgresExtensions
{
    public static void AddPostgresDbContext<TContext>(this IServiceCollection services, PostgresConfig config) 
        where TContext : DbContext
    {
        services.AddSingleton(config);
        
        services.AddDbContext<TContext>(options =>
        {
            options.UseNpgsql(config.ConnectionString, builder =>
            {
                builder.MigrationsAssembly(config.Schema);
                builder.MigrationsHistoryTable("__EFMigrationsHistory", config.Schema);
            });
        });
    }
}