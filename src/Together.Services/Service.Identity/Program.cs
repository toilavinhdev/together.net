using Infrastructure.PostgreSQL;
using Infrastructure.SharedKernel;
using Infrastructure.SharedKernel.Extensions;
using Service.Identity;
using Service.Identity.Domain;
using Service.Identity.gRPC;

var (builder, appSettings) = WebApplicationBuilderExtensions.CreateCoreBuilder<AppSettings>(args);

var services = builder.Services;
services.AddSharedKernel<Program>(appSettings);
services.AddPostgresDbContext<IdentityContext>(appSettings.PostgresConfig);
services.AddGrpc();

var app = builder.Build();
app.UseSharedKernel(appSettings);
app.UseStaticFiles();
app.UseGrpc(appSettings.GrpcEndpoints.ServiceIdentity, endpoint =>
{
    endpoint.MapGrpcService<UserGrpcService>();
});
await IdentityContextInitialization.SeedAsync(app.Services);
app.Run();