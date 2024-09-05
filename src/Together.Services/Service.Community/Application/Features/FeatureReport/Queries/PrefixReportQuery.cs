using Infrastructure.SharedKernel.Mediator;
using Microsoft.EntityFrameworkCore;
using Service.Community.Application.Features.FeatureReport.Responses;
using Service.Community.Domain;

namespace Service.Community.Application.Features.FeatureReport.Queries;

public sealed class PrefixReportQuery : IBaseRequest<List<PrefixReportResponse>>
{
    internal class Handler(IHttpContextAccessor httpContextAccessor, CommunityContext context) 
        : BaseRequestHandler<PrefixReportQuery, List<PrefixReportResponse>>(httpContextAccessor)
    {
        protected override async Task<List<PrefixReportResponse>> HandleAsync(PrefixReportQuery request, CancellationToken ct)
        {
            var totalPost = await context.Posts.LongCountAsync(ct);
            
            var report = await context.Prefixes
                .Include(prefix => prefix.Posts)
                .OrderByDescending(prefix => prefix.Posts!.LongCount())
                .Select(prefix => new PrefixReportResponse
                {
                    Id = prefix.Id,
                    Name = prefix.Name,
                    Background = prefix.Background,
                    Foreground = prefix.Foreground,
                    TotalPost = prefix.Posts!.LongCount(),
                    Percentage = Math.Round((double)prefix.Posts!.LongCount() / totalPost * 100, 2) 
                })
                .ToListAsync(ct);

            var totalPostWithoutPrefix = await context.Posts.LongCountAsync(post => post.PrefixId == null, ct);

            report = [..report, new PrefixReportResponse
            {
                Id = Guid.Empty,
                Background = "#ECF1F4",
                Foreground = "#000000",
                Name = "WithoutPrefix",
                TotalPost = totalPostWithoutPrefix,
                Percentage = Math.Round((double)totalPostWithoutPrefix / totalPost * 100, 2) 
            }];
            
            return report;
        }
    }
}