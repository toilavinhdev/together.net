using Infrastructure.Redis;
using Infrastructure.SharedKernel.Extensions;
using Infrastructure.SharedKernel.Mediator;
using Microsoft.EntityFrameworkCore;
using Service.Identity.Domain;

namespace Service.Identity.Application.Features.FeatureReport.Queries;

public class StatisticsQuery(List<string>? metrics) : IBaseRequest<Dictionary<string, object>>
{
    private List<string>? Metrics { get; set; } = metrics;
    
    internal class Handler(IHttpContextAccessor httpContextAccessor,
        IdentityContext context,
        IRedisService redisService) 
        : BaseRequestHandler<StatisticsQuery, Dictionary<string, object>>(httpContextAccessor)
    {
        protected override async Task<Dictionary<string, object>> HandleAsync(StatisticsQuery request, CancellationToken ct)
        {
            var data = new Dictionary<string, object>();

            if (request.Metrics is null) return data;
            
            if (request.Metrics.Contains("totalOnlineUser"))
            {
                var totalOnlineUser = await redisService.SetLengthAsync(RedisKeys.SocketOnlineUsers());
                data.Add("totalOnlineUser", totalOnlineUser);
            }
            
            if (request.Metrics.Contains("totalUser"))
            {
                var totalUser = await context.Users.LongCountAsync(ct);
                data.Add("totalUser", totalUser);
            }
            
            if (request.Metrics.Contains("totalUserToday"))
            {
                var start = DateTimeOffset.UtcNow.StartOfDayUtc();
                var end = DateTimeOffset.UtcNow.EndOfDayUtc();
                var totalUserToday = await context.Users.LongCountAsync(u => u.CreatedAt >= start && u.CreatedAt <= end, ct);
                data.Add("totalUserToday", totalUserToday);
            }
            
            if (request.Metrics.Contains("newMember"))
            {
                var newMember = await context.Users
                    .OrderByDescending(u => u.CreatedAt)
                    .FirstOrDefaultAsync(ct);
                if(newMember is not null) data.Add("newMember", newMember.UserName);
            }

            return data;
        }
    }
}