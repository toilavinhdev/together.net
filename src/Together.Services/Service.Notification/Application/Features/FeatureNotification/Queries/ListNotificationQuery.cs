using FluentValidation;
using Grpc.Net.Client;
using Infrastructure.Redis;
using Infrastructure.SharedKernel.BusinessObjects;
using Infrastructure.SharedKernel.Extensions;
using Infrastructure.SharedKernel.Mediator;
using Infrastructure.SharedKernel.Protos.Identity;
using Infrastructure.SharedKernel.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Service.Notification.Application.Features.FeatureNotification.Responses;
using Service.Notification.Domain;

namespace Service.Notification.Application.Features.FeatureNotification.Queries;

public sealed class ListNotificationQuery : IBaseRequest<ListNotificationResponse>, IPaginationRequest
{
    public int PageIndex { get; set; }
    
    public int PageSize { get; set; }
    
    public class Validator : AbstractValidator<ListNotificationQuery>
    {
        public Validator()
        {
            Include(new PaginationValidator());
        }
    }
    
    internal class Handler(IHttpContextAccessor httpContextAccessor, 
        NotificationContext context,
        AppSettings appSettings,
        IRedisService redisService) 
        : BaseRequestHandler<ListNotificationQuery, ListNotificationResponse>(httpContextAccessor)
    {
        protected override async Task<ListNotificationResponse> HandleAsync(ListNotificationQuery request, CancellationToken ct)
        {
            var queryable = context.Notifications
                .Where(n => n.ReceiverId == UserClaimsPrincipal.Id)
                .AsQueryable();

            var count = await queryable.LongCountAsync(ct);
            
            var notifications = await queryable
                .OrderByDescending(n => n.CreatedAt)
                .Paging(request.PageIndex, request.PageSize)
                .Select(n => new NotificationViewModel
                {
                    Id = n.Id,
                    SubId = n.SubId,
                    CreatedAt = n.CreatedAt,
                    ModifiedAt = n.ModifiedAt,
                    Subject = new GeneralUser
                    {
                        Id = n.SubjectId,
                        UserName = string.Empty,
                        Avatar = null,
                        IsOfficial = false,
                    },
                    Status = n.Status,
                    Type = n.Type,
                    DirectObjectId = n.DirectObjectId,
                    IndirectObjectId = n.IndirectObjectId,
                    PrepositionalObjectId = n.PrepositionalObjectId
                })
                .ToListAsync(ct);
            
            // Attach subjects
            foreach (var notification in notifications)
            {
                var cachedUser = await redisService.StringGetAsync<IdentityPrivilege>(
                        RedisKeys.Identity<IdentityPrivilege>(notification.Subject.Id));
                if (cachedUser is not null)
                {
                    notification.Subject.UserName = cachedUser.UserName;
                    notification.Subject.Avatar = cachedUser.Avatar;
                    notification.Subject.IsOfficial = cachedUser.IsOfficial;
                }
                else
                {
                    var channelOptions = new GrpcChannelOptions { HttpHandler = new HttpClientHandler { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator } };
                    var channel = GrpcChannel.ForAddress(appSettings.GrpcEndpoints.ServiceIdentity, channelOptions);
                    var client = new UserGrpcServerService.UserGrpcServerServiceClient(channel);
                    var response = await client.GetGeneralUserGrpcAsync(new GetGeneralUserGrpcRequest { Id = notification.Subject.Id.ToString() }, cancellationToken: ct);
                    notification.Subject.UserName = response.Data.UserName;
                    notification.Subject.Avatar = response.Data.Avatar;
                    notification.Subject.IsOfficial = response.Data.IsOfficial;
                }
            }
            
            return new ListNotificationResponse
            {
                Pagination = new Pagination(request.PageIndex, request.PageSize, count),
                Result = notifications,
            };
        }
    }
}