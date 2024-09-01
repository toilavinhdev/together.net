using Infrastructure.Logging;
using Infrastructure.PostgreSQL;
using Infrastructure.SharedKernel;
using Infrastructure.SharedKernel.Extensions;
using Service.Identity;
using Service.Identity.Domain;

var builder = WebApplication.CreateBuilder(args);
builder.SetupEnvironment<AppSettings>(out var appSettings);
builder.SetupSerilog();

var services = builder.Services;
services.AddSharedKernel<Program>(appSettings);
services.AddPostgresDbContext<IdentityContext>(appSettings.PostgresConfig);

var app = builder.Build();
app.UseSharedKernel(appSettings);
app.UseStaticFiles();
await IdentityContextInitialization.SeedAsync(app.Services);
app.Run();