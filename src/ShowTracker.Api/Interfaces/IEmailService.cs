namespace ShowTracker.Api.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string toName, string subject, string body);
}
