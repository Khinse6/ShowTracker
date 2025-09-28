using ShowTracker.Api.Interfaces;

namespace ShowTracker.Api.Services;

public class ConsoleEmailService : IEmailService
{
    private readonly ILogger<ConsoleEmailService> _logger;

    public ConsoleEmailService(ILogger<ConsoleEmailService> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(string toEmail, string toName, string subject, string body)
    {
        _logger.LogInformation("--- Sending Email ---");
        _logger.LogInformation("To: {ToName} <{ToEmail}>", toName, toEmail);
        _logger.LogInformation("Subject: {Subject}", subject);
        _logger.LogInformation("Body: \n{Body}", body);
        _logger.LogInformation("--- Email Sent ---");

        return Task.CompletedTask;
    }
}
