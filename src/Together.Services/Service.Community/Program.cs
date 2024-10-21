using Infrastructure.PostgreSQL;
using Infrastructure.SharedKernel;
using Infrastructure.SharedKernel.Extensions;
using Service.Community;
using Service.Community.Domain;

var (builder, appSettings) = WebApplicationBuilderExtensions.CreateCoreBuilder<AppSettings>(args);

var services = builder.Services;
services.AddSharedKernel<Program>(appSettings);
services.AddPostgresDbContext<CommunityContext>(appSettings.PostgresConfig);
services.AddGrpc();

var app = builder.Build();
app.UseSharedKernel(appSettings);
app.UseGrpc(appSettings.GrpcEndpoints.ServiceCommunity, _ => {});
await CommunityContextInitialization.SeedAsync(app.Services);
app.Run();