using System.Net.Http.Headers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Logging;

public sealed class CorrelationLogger;

public static class CorrelationIdExtensions
{
    private const string XCorrelationId = "X-Correlation-ID";

    public static string GenerateCorrelationId() => Guid.NewGuid().ToString();

    public static void UseCorrelationId(this IApplicationBuilder app)
        => app.Use(async (ctx, next) =>
        {
            if (!ctx.Request.Headers.TryGetValue(XCorrelationId, out var correlationId))
            {
                correlationId = GenerateCorrelationId();
            }
            ctx.Items[XCorrelationId] = correlationId.ToString();
            var logger = ctx.RequestServices.GetRequiredService<ILogger<CorrelationLogger>>();
            using (logger.BeginScope("{@CorrelationId}", correlationId!))
            {
                await next(ctx);
            }
        });

    public static string? GetCorrelationId(this HttpContext context)
        => context.Items.TryGetValue(XCorrelationId, out var correlationId) ? correlationId as string : null;

    public static void AddCorrelationId(this HttpRequestHeaders headers, string correlationId)
        => headers.TryAddWithoutValidation(XCorrelationId, correlationId);
}