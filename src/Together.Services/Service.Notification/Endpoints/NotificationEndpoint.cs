using Infrastructure.SharedKernel.Extensions;
using Infrastructure.SharedKernel.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Service.Notification.Application.Features.FeatureNotification.Commands;
using Service.Notification.Application.Features.FeatureNotification.Queries;
using Service.Notification.Application.Features.FeatureNotification.Responses;

namespace Service.Notification.Endpoints;

public sealed class NotificationEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/notification").WithTags("Notification");

        group.MapGet("/list", ListNotification);
        
        group.MapGet("/unread-count", CountNotificationUnread);
        
        group.MapPost("/mark-read", MarkReadNotification);
        
        group.MapPost("/mark-all-read", MarkAllReadNotification);
    }

    [Authorize]
    private static Task<BaseResponse<ListNotificationResponse>> ListNotification(ISender sender, [AsParameters] ListNotificationQuery query)
        => sender.Send(query);
    
    [Authorize]
    private static Task<BaseResponse<int>> CountNotificationUnread(ISender sender)
        => sender.Send(new CountUnreadNotificationQuery());
    
    [Authorize]
    private static Task<BaseResponse> MarkReadNotification(ISender sender, MarkReadNotificationCommand command)
        => sender.Send(command);
    
    [Authorize]
    private static Task<BaseResponse> MarkAllReadNotification(ISender sender, MarkAllReadNotificationCommand command)
        => sender.Send(command);
}