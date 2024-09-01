using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Infrastructure.RabbitMQ;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public static class RabbitMQExtensions
{
    public static void AddRabbitMQ(this IServiceCollection services, RabbitMQConfig config)
    {
        services.AddSingleton<IRabbitMQClient>(serviceProvider =>
        {
            var connectionFactory = new ConnectionFactory
            {
                HostName = config.Host,
                Port = config.Port,
                UserName = config.UserName,
                Password = config.Password,
                DispatchConsumersAsync = true
            };

            var logger = serviceProvider.GetRequiredService<ILogger<RabbitMQClient>>();
            
            return new RabbitMQClient(connectionFactory, config, logger);
        });
    }
}