using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Infrastructure.Redis;

public static class RedisExtensions
{
    public static void AddRedis(this IServiceCollection services, string configuration)
    {
        services.AddSingleton<IConnectionMultiplexer>(_ =>
        {
            var lazyConnection = new Lazy<ConnectionMultiplexer>(
                () => ConnectionMultiplexer.Connect(configuration));
            return lazyConnection.Value;
        });
        
        services.AddSingleton<IRedisService, RedisService>();
    }
}