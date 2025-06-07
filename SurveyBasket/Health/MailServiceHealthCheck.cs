using MailKit.Net.Smtp;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SurveyBasket.Settings;

namespace SurveyBasket.Health;

public class MailServiceHealthCheck(IOptions<MailSettings> mailSettings) : IHealthCheck
{
    private readonly MailSettings _mailSettings = mailSettings.Value;

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
		try
		{
			using var client = new MailKit.Net.Smtp.SmtpClient();

			client.Connect(_mailSettings.Host, _mailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls,cancellationToken);
            client.Authenticate(_mailSettings.Mail, _mailSettings.Password , cancellationToken);

            return await Task.FromResult(HealthCheckResult.Healthy());
		}
		catch (Exception exp)
		{
            return await Task.FromResult(HealthCheckResult.Unhealthy("Mail service is unhealthy", exp));
        }
    }
}
