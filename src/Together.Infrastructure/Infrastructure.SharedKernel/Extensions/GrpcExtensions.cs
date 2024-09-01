using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Infrastructure.SharedKernel.Extensions;

public static class GrpcExtensions
{
    public static void UseGrpc(this IApplicationBuilder app, 
        string https, 
        Action<IEndpointRouteBuilder> endpointBuilder)
    {
        var port = new Uri(https).Port;
        
        app.UseWhen(ctx => ctx.Connection.LocalPort == port, appBuilder =>
        {
            appBuilder.UseRouting();
            appBuilder.UseEndpoints(endpointBuilder);
        });
    }
}