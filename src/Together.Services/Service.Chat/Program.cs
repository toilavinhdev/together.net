using Infrastructure.PostgreSQL;
using Infrastructure.SharedKernel;
using Infrastructure.SharedKernel.Extensions;
using Service.Chat;
using Service.Chat.Domain;

var (builder, appSettings) = WebApplicationBuilderExtensions.CreateCoreBuilder<AppSettings>(args);

var services = builder.Services;
services.AddSharedKernel<Program>(appSettings);
services.AddPostgresDbContext<ChatContext>(appSettings.PostgresConfig);

var app = builder.Build();
app.UseSharedKernel(appSettings);
app.UseGrpc(appSettings.GrpcEndpoints.ServiceChat, _ => {});
await ChatContextInitialization.SeedAsync(app.Services);
app.Run();