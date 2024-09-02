using Infrastructure.PostgreSQL;
using Infrastructure.SharedKernel.BusinessObjects;
using Infrastructure.SharedKernel.Enums;
using Infrastructure.SharedKernel.ValueObjects;
using Service.Notification.Domain.Enums;

namespace Service.Notification.Application.Features.FeatureNotification.Responses;

public class ListNotificationResponse : PaginationResult<NotificationViewModel>;

public class NotificationViewModel : TimeTrackingEntity
{
    public GeneralUser Subject { get; set; } = default!;
    
    public NotificationType Type { get; set; }
    
    public Guid DirectObjectId { get; set; }
    
    public Guid? IndirectObjectId { get; set; }
    
    public Guid? PrepositionalObjectId { get; set; }
    
    public NotificationStatus Status { get; set; }
}