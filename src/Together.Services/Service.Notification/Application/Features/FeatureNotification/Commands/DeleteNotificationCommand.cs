using Infrastructure.SharedKernel.Enums;
using Infrastructure.SharedKernel.Mediator;
using Microsoft.EntityFrameworkCore;
using Service.Notification.Domain;

namespace Service.Notification.Application.Features.FeatureNotification.Commands;

public sealed class DeleteNotificationCommand : IBaseRequest
{
    public Guid SubjectId { get; set; }
    
    public NotificationType Type { get; set; }
    
    public Guid DirectObjectId { get; set; }
    
    internal class Handler(NotificationContext context,
        IHttpContextAccessor httpContextAccessor) : BaseRequestHandler<DeleteNotificationCommand>(httpContextAccessor)
    {
        protected override async Task HandleAsync(DeleteNotificationCommand request, CancellationToken ct)
        {
            var notification = await context.Notifications
                .FirstOrDefaultAsync(n =>
                        n.SubjectId == request.SubjectId &&
                        n.Type == request.Type &&
                        n.DirectObjectId == request.DirectObjectId,
                    ct);
            
            if (notification is null) return;

            context.Notifications.Remove(notification);

            await context.SaveChangesAsync(ct);
        }
    }
}