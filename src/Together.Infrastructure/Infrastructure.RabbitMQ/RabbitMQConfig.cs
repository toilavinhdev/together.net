using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.RabbitMQ;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public sealed class RabbitMQConfig
{
    public string Host { get; set; } = default!;

    public int Port { get; set; }

    public string UserName { get; set; } = default!;

    public string Password { get; set; } = default!;

    public string? Exchange { get; set; }
}