namespace Infrastructure.RabbitMQ.Events;

public class CreateNotificationEvent : RabbitMQEvent
{
    public Guid ReceiverId { get; set; }
    
    public Guid SubjectId { get; set; }
    
    public int NotificationType { get; set; }
    
    public Guid DirectObjectId { get; set; }
    
    public Guid? IndirectObjectId { get; set; }
    
    public Guid? PrepositionalObjectId { get; set; }
}