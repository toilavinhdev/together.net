using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.WebSocket;

public static class WebSocketExtensions
{
    public static void AddWebSocketHandlers<TAssembly>(this IServiceCollection services)
    {
        services.AddTransient<ConnectionManager>();
        
        typeof(TAssembly).Assembly.ExportedTypes
            .Where(t => t.GetTypeInfo().BaseType == typeof(WebSocketHandler))
            .ToList()
            .ForEach(t => services.AddSingleton(t));
    }
    
    public static void MapWebSocketHandler<THandler>(this WebApplication app, PathString path)
        where THandler : WebSocketHandler
    {
        var handlerService = app.Services.GetService<THandler>();

        app.Map(path, c => c.UseMiddleware<WebSocketMiddleware>(handlerService));
    }
}