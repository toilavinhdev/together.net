using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Infrastructure.Logging;
using Infrastructure.SharedKernel.Extensions;
using Infrastructure.SharedKernel.Protos.Identity;
using MediatR;
using Service.Identity.Application.Features.FeatureReport.Queries;
using Service.Identity.Application.Features.FeatureUser.Queries;

namespace Service.Identity.gRPC;

public sealed class UserGrpcService(ISender sender, ILogger<UserGrpcService> logger) : UserGrpcServerService.UserGrpcServerServiceBase
{
    public override Task<PingGrpcResponse> PingGrpc(Empty request, ServerCallContext context)
    {
        return Task.FromResult(new PingGrpcResponse { Message = "Grpc.Identity - Pong" });
    }

    public override async Task<GetGeneralUserGrpcResponse> GetGeneralUserGrpc(GetGeneralUserGrpcRequest request, ServerCallContext context)
    {
        var correlationId = context.RequestHeaders
            .FirstOrDefault(h => h.Key == CorrelationIdExtensions.XCorrelationId)?
            .Value;
        using var scope = logger.BeginScope("{@CorrelationId}", correlationId);
        logger.LogInformation(
            "Request [gRPC] {MethodName}: {@Value}",
            nameof(UserStatisticsGrpc),
            request
        );

        var user = await sender.Send(new GetGeneralUserQuery(request.Id.ToGuid()));

        if (user is null) return new GetGeneralUserGrpcResponse { Data = null };

        return new GetGeneralUserGrpcResponse
        {
            Data = new GeneralUserGrpc
            {
                Id = user.Id.ToString(),
                UserName = user.UserName,
                Avatar = user.Avatar,
                IsOfficial = user.IsOfficial
            }
        };
    }

    public override async Task<UserStatisticsGrpcResponse> UserStatisticsGrpc(UserStatisticsGrpcRequest request, ServerCallContext context)
    {
        logger.LogInformation(
            "Request [gRPC] {MethodName}: {@Value}",
            nameof(UserStatisticsGrpc),
            request
        );

        var statistics = await sender.Send(new StatisticsQuery(request.Metrics.ToList()));

        var metricPairs = statistics.Data.Select(pair => new MetricPair
        {
            Metric = pair.Key,
            Value = pair.Value.ToString(),
        }).ToList();
        
        return new UserStatisticsGrpcResponse
        {
            Data =
            {
                metricPairs
            }
        };
    }
}