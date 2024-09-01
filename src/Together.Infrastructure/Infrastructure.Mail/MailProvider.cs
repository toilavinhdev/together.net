using System.Net.Mail;
using MimeKit;

namespace Infrastructure.Mail;

public static class MailProvider
{
    public static async Task SmtpSendAsync(MailConfig config, MailForm form)
    {
        var mimeMessage = new MimeMessage();
        
        mimeMessage.Sender = new MailboxAddress(config.DisplayName, config.Mail);
        
        mimeMessage.From.Add(new MailboxAddress(config.DisplayName, config.Mail));
        
        mimeMessage.To.Add(MailboxAddress.Parse(form.To));
        
        mimeMessage.Subject = form.Title;
        
        mimeMessage.Body = new TextPart("html")
        {
            Text = form.Body
        };
        
        using var smtp = new MailKit.Net.Smtp.SmtpClient();
        
        try {
            await smtp.ConnectAsync(config.Host, config.Port, false);
            
            await smtp.AuthenticateAsync (config.Mail, config.Password);
            
            await smtp.SendAsync(mimeMessage);
            
            await smtp.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            throw new SmtpException("Send mail failed", ex);
        }
    }
}