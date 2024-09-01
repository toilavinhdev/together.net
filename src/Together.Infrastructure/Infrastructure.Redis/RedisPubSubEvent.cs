namespace Infrastructure.Redis;

public abstract class RedisPubSubEvent
{
    public Guid EventId { get; set; } = Guid.NewGuid();
}