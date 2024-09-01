namespace Infrastructure.Mail;

public sealed class MailForm
{
    public string To { get; set; } = default!;

    public string Title { get; set; } = default!;

    public string Body { get; set; } = default!;
}