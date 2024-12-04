using Infrastructure.SharedKernel.Authorization;
using Infrastructure.SharedKernel.Constants;
using Infrastructure.SharedKernel.Extensions;
using Infrastructure.SharedKernel.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Service.Community.Application.Features.FeatureReport.Queries;
using Service.Community.Application.Features.FeatureReport.Responses;

namespace Service.Community.Endpoints;

public sealed class ReportEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/report").WithTags("Report");
        
        group.MapGet("/prefix", PrefixReport);

        group.MapPost("/statistics", Statistics);

        group.MapGet("/daily-post", DailyPostReport);
    }

    [Authorize]
    private static Task<BaseResponse<Dictionary<string, object>>> Statistics(ISender sender, StatisticsQuery query)
        => sender.Send(query);
    
    [AccessControl(Policies.Management.ViewDashboard)]
    private static Task<BaseResponse<List<PrefixReportResponse>>> PrefixReport(ISender sender)
        => sender.Send(new PrefixReportQuery());

    [AccessControl(Policies.Management.ViewDashboard)]
    private static Task<BaseResponse<List<DailyPostReportResponse>>> DailyPostReport(ISender sender, [AsParameters] DailyPostReportQuery query)
        => sender.Send(query);
}