using Infrastructure.SharedKernel.Mediator;
using Microsoft.EntityFrameworkCore;
using Service.Notification.Domain;
using Service.Notification.Domain.Enums;

namespace Service.Notification.Application.Features.FeatureNotification.Queries;

public sealed class CountUnreadNotificationQuery : IBaseRequest<int>
{
    internal class Handler(IHttpContextAccessor httpContextAccessor, NotificationContext context) 
        : BaseRequestHandler<CountUnreadNotificationQuery, int>(httpContextAccessor)
    {
        protected override async Task<int> HandleAsync(CountUnreadNotificationQuery request, CancellationToken ct)
        {
            return await context.Notifications
                .CountAsync(n => 
                    n.ReceiverId == UserClaimsPrincipal.Id && 
                    n.Status == NotificationStatus.Unread, 
                    ct);
        }
    }
}