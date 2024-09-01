using Infrastructure.SharedKernel.Extensions;
using Infrastructure.SharedKernel.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Service.Community.Application.Features.FeatureStatistics.Queries;

namespace Service.Community.Endpoints;

public sealed class ReportEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/report").WithTags("Report");

        group.MapPost("/statistics", Statistics);
    }

    [Authorize]
    private static Task<BaseResponse<Dictionary<string, object>>> Statistics(ISender sender, StatisticsQuery query)
        => sender.Send(query);
}