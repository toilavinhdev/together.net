using Infrastructure.SharedKernel.Extensions;
using Infrastructure.SharedKernel.Mediator;
using Microsoft.EntityFrameworkCore;
using Service.Community.Domain;

namespace Service.Community.Application.Features.FeatureStatistics.Queries;

public sealed class StatisticsQuery : IBaseRequest<Dictionary<string, object>>
{
    public List<string>? Metrics { get; set; }
    
    internal class Handle(IHttpContextAccessor httpContextAccessor, CommunityContext context) 
        : BaseRequestHandler<StatisticsQuery, Dictionary<string, object>>(httpContextAccessor)
    {
        protected override async Task<Dictionary<string, object>> HandleAsync(StatisticsQuery request, CancellationToken ct)
        {
            var data = new Dictionary<string, object>();

            if (request.Metrics is null) return data;

            if (request.Metrics.Contains("totalTopic"))
            {
                var totalTopic = await context.Topics.LongCountAsync(ct);
                data.Add("totalTopic", totalTopic);
            }
            
            if (request.Metrics.Contains("totalPost"))
            {
                var totalPost = await context.Posts.LongCountAsync(ct);
                data.Add("totalPost", totalPost);
            }
            
            if (request.Metrics.Contains("totalPostToday"))
            {
                var start = DateTimeOffset.UtcNow.StartOfDayUtc();
                var end = DateTimeOffset.UtcNow.EndOfDayUtc();
                var totalPostToday = await context.Posts.LongCountAsync(p => p.CreatedAt >= start && p.CreatedAt <= end, ct);
                data.Add("totalPostToday", totalPostToday);
            }
            
            if (request.Metrics.Contains("totalReply"))
            {
                var totalReply = await context.Replies.LongCountAsync(ct);
                data.Add("totalReply", totalReply);
            }
            
            if (request.Metrics.Contains("totalReplyToday"))
            {
                var start = DateTimeOffset.UtcNow.StartOfDayUtc();
                var end = DateTimeOffset.UtcNow.EndOfDayUtc();
                var totalReplyToday = await context.Replies.LongCountAsync(r => r.CreatedAt >= start && r.CreatedAt <= end, ct);
                data.Add("totalReplyToday", totalReplyToday);
            }
            
            return data;
        }
    }
}