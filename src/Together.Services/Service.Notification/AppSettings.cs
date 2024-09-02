using Infrastructure.PostgreSQL;
using Infrastructure.SharedKernel.ValueObjects;

namespace Service.Notification;

public sealed class AppSettings : BaseSettings
{
    public PostgresConfig PostgresConfig { get; set; } = default!;
}