using Infrastructure.SharedKernel.Mediator;
using Microsoft.EntityFrameworkCore;
using Service.Notification.Domain;
using Service.Notification.Domain.Enums;

namespace Service.Notification.Application.Features.FeatureNotification.Commands;

public sealed class MarkAllReadNotificationCommand : IBaseRequest
{
    internal class Handler(IHttpContextAccessor httpContextAccessor, NotificationContext context) 
        : BaseRequestHandler<MarkAllReadNotificationCommand>(httpContextAccessor)
    {
        protected override async Task HandleAsync(MarkAllReadNotificationCommand request, CancellationToken ct)
        {
            await context.Notifications
                .Where(n => n.ReceiverId == UserClaimsPrincipal.Id)
                .ExecuteUpdateAsync(prop => prop
                        .SetProperty(n => n.Status, NotificationStatus.Read),
                    ct);
        }
    }
}