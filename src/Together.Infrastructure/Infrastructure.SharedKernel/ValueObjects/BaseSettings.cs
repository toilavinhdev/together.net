using Infrastructure.Logging;
using Infrastructure.RabbitMQ;

namespace Infrastructure.SharedKernel.ValueObjects;

public class BaseSettings
{
    public Metadata Metadata { get; set; } = default!;

    public LoggingConfig LoggingConfig { get; set; } = default!;
    
    public GrpcEndpoints GrpcEndpoints { get; set; } = default!;

    public JwtConfig JwtConfig { get; set; } = default!;
    
    public string RedisConfiguration { get; set; } = default!;

    public RabbitMQConfig RabbitMqConfig { get; set; } = default!;
}

public sealed class Metadata
{
    public string Host { get; set; } = default!;
    
    public string Name { get; set; } = default!;

    public string EndpointPrefix { get; set; } = default!;
}

public sealed class JwtConfig
{
    public string TokenSigningKey { get; set; } = default!;
    
    public int AccessTokenDurationInMinutes { get; set; }
    
    public int RefreshTokenDurationInDays { get; set; }

    public string? Issuer { get; set; }
    
    public string? Audience { get; set; }
}

public sealed class GrpcEndpoints
{
    public string ServiceIdentity { get; set; } = default!;
    
    public string ServiceCommunity { get; set; } = default!;
    
    public string ServiceChat { get; set; } = default!;
    
    public string ServiceNotification { get; set; } = default!;
    
    public string ServiceSocket { get; set; } = default!;
}