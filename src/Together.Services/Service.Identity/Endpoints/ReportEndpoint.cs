using Infrastructure.SharedKernel.Authorization;
using Infrastructure.SharedKernel.Constants;
using Infrastructure.SharedKernel.Extensions;
using Infrastructure.SharedKernel.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Service.Identity.Application.Features.FeatureReport.Queries;
using Service.Identity.Application.Features.FeatureReport.Responses;

namespace Service.Identity.Endpoints;

public sealed class ReportEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/v1/report").WithTags("Report");

        group.MapPost("/statistics", Statistics);
        
        group.MapGet("/daily-user", DailyUserReport);
    }

    [Authorize]
    private static Task<BaseResponse<Dictionary<string, object>>> Statistics(ISender sender, StatisticsQuery query)
        => sender.Send(query);
    
    [AccessControl(Policies.Management.ViewDashboard)]
    private static Task<BaseResponse<List<DailyUserReportResponse>>> DailyUserReport(ISender sender, [AsParameters] DailyUserReportQuery query)
        => sender.Send(query);
}