using System.Diagnostics;
using Infrastructure.SharedKernel.Extensions;
using Infrastructure.SharedKernel.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Infrastructure.SharedKernel.Mediator;

public sealed class LoggingBehavior<TRequest, TResponse>(ILogger<TRequest> logger, BaseSettings baseSettings)
    : IPipelineBehavior<TRequest, TResponse> 
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "[{ServiceName}].[{RequestName}]: {RequestValue}",
            baseSettings.Metadata.Name,
            request.GetType().Name,
            request.ToJson()
        );
        
        var timer = new Stopwatch();
        
        timer.Start();
        var response = await next().ConfigureAwait(false);
        timer.Stop();
        
        var elapsedMilliseconds = timer.ElapsedMilliseconds;

        logger.LogInformation(
            "[{ServiceName}].[{RequestName}] ({Elapsed}ms)",
            baseSettings.Metadata.Name,
            request.GetType().Name,
            elapsedMilliseconds
        );

        return response;
    }
}