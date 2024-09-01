using Infrastructure.Logging;
using Infrastructure.PostgreSQL;
using Infrastructure.SharedKernel;
using Infrastructure.SharedKernel.Extensions;
using Service.Community;
using Service.Community.Domain;

var builder = WebApplication.CreateBuilder(args);
builder.SetupEnvironment<AppSettings>(out var appSettings);
builder.SetupSerilog();

var services = builder.Services;
services.AddSharedKernel<Program>(appSettings);
services.AddPostgresDbContext<CommunityContext>(appSettings.PostgresConfig);

var app = builder.Build();
app.UseSharedKernel(appSettings);
await CommunityContextInitialization.SeedAsync(app.Services);
app.Run();