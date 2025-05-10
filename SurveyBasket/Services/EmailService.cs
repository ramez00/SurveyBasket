using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;
using SurveyBasket.Settings;

namespace SurveyBasket.Services;

public class EmailService(IOptions<MailSettings> mailSettings,ILogger<EmailService> logger) : IEmailSender
{
    private readonly MailSettings _mailSettings = mailSettings.Value;
    private readonly ILogger<EmailService> _logger = logger;

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        // Implement your email sending logic here using the provided email, subject, and htmlMessage.

        var message = new MimeMessage
        {
            Sender = MailboxAddress.Parse(_mailSettings.Mail),
            Subject = subject,
        };

        message.To.Add(MailboxAddress.Parse(email));

        var builder = new BodyBuilder
        {
            HtmlBody = htmlMessage,
        };
        message.Body = builder.ToMessageBody();

        _logger.LogInformation("Sending Email to {email}", email);

        using (var client = new MailKit.Net.Smtp.SmtpClient())
        {
            await client.ConnectAsync(_mailSettings.Host, _mailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_mailSettings.Mail, _mailSettings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
