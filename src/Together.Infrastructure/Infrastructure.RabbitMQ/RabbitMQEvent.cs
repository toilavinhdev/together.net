using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.RabbitMQ;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public abstract class RabbitMQEvent
{
    public Guid EventId { get; set; } = Guid.NewGuid();
}