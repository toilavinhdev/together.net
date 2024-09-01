using Infrastructure.Logging;
using Infrastructure.SharedKernel;
using Infrastructure.SharedKernel.Extensions;
using Service.Identity;

var builder = WebApplication.CreateBuilder(args);
builder.SetupEnvironment<AppSettings>(out var appSettings);
builder.SetupSerilog();

var services = builder.Services;
services.AddSharedKernel<Program>(appSettings);

var app = builder.Build();
app.UseSharedKernel(appSettings);
app.UseStaticFiles();
app.Run();