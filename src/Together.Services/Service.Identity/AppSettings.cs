using Infrastructure.Mail;
using Infrastructure.PostgreSQL;
using Infrastructure.SharedKernel.ValueObjects;

namespace Service.Identity;

public sealed class AppSettings : BaseSettings
{
    public PostgresConfig PostgresConfig { get; set; } = default!;
    
    public MailConfig MailConfig { get; set; } = default!;
    
    public int ForgotPasswordTokenDurationInHours { get; set; }
    
    public GoogleConfig GoogleConfig { get; set; } = default!;
}

public sealed class GoogleConfig
{
    public string ClientId { get; set; } = default!;
}