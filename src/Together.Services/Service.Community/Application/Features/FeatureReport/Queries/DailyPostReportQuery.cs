using FluentValidation;
using Infrastructure.SharedKernel.Extensions;
using Infrastructure.SharedKernel.Mediator;
using Microsoft.EntityFrameworkCore;
using Service.Community.Application.Features.FeatureReport.Responses;
using Service.Community.Domain;

namespace Service.Community.Application.Features.FeatureReport.Queries;

public sealed class DailyPostReportQuery : IBaseRequest<List<DailyPostReportResponse>>
{
    public DateTimeOffset From { get; set; }

    public DateTimeOffset To { get; set; }

    public class Validator : AbstractValidator<DailyPostReportQuery>
    {
        public Validator()
        {
            RuleFor(x => x.From).NotEmpty();
            RuleFor(x => x.To)
                .NotEmpty()
                .GreaterThanOrEqualTo(x => x.From);
        }
    }

    internal class Handler(IHttpContextAccessor httpContextAccessor, CommunityContext context)
        : BaseRequestHandler<DailyPostReportQuery, List<DailyPostReportResponse>>(httpContextAccessor)
    {
        protected override async Task<List<DailyPostReportResponse>> HandleAsync(DailyPostReportQuery request, CancellationToken ct)
        {
            var report = new List<DailyPostReportResponse>();

            var days = DateTimeExtensions.GetDaysInRange(request.From, request.To);

            foreach (var day in days)
            {
                var totalPost = await context.Posts
                    .LongCountAsync(u => u.CreatedAt >= day.StartOfDayUtc() && u.CreatedAt <= day.EndOfDayUtc(), ct);

                report.Add(new DailyPostReportResponse
                {
                    Day = day,
                    TotalPost = totalPost
                });
            }

            return report;
        }
    }
}