using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Infrastructure.SharedKernel.Mediator;

public sealed class PerformanceBehavior<TRequest, TResponse>(ILogger<TRequest> logger) 
    : IPipelineBehavior<TRequest, TResponse> 
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        logger.LogInformation("Start processing request {Request}", typeof(TRequest).Name);
        
        var timer = new Stopwatch();
        
        timer.Start();
        var response = await next().ConfigureAwait(false);
        timer.Stop();
        
        var elapsedMilliseconds = timer.ElapsedMilliseconds;

        if (elapsedMilliseconds <= 5000)
        {
            logger.LogInformation("Request {Request} completed in {Elapsed} milliseconds", 
                typeof(TRequest).Name, elapsedMilliseconds);
        }
        else
        {
            logger.LogWarning("Request long time running: {Request} {Elapsed} milliseconds", 
                typeof(TRequest).Name, elapsedMilliseconds);
        }

        return response;
    }
}