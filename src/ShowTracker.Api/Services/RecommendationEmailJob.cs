using Microsoft.EntityFrameworkCore;
using ShowTracker.Api.Data;
using System.Text;

namespace ShowTracker.Api.Services;

public class RecommendationEmailJob : IHostedService, IDisposable
{
    private readonly ILogger<RecommendationEmailJob> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private Timer? _timer;

    public RecommendationEmailJob(ILogger<RecommendationEmailJob> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Recommendation Email Job is starting.");

        // Run once a day. For testing, you could change this to a shorter interval.
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromDays(1));

        return Task.CompletedTask;
    }

    private async void DoWork(object? state)
    {
        try
        {
            // We don't want the timer to be blocked, so we execute the work and don't await it.
            // The try-catch is essential as unhandled exceptions in async void methods can crash the process.
            await TriggerManually(CancellationToken.None);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred in the recommendation email job's timer.");
        }
    }

    public async Task TriggerManually(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Recommendation Email Job is running (manual trigger).");

        using var scope = _scopeFactory.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<ShowStoreContext>();
        var recommendationService = scope.ServiceProvider.GetRequiredService<IRecommendationService>();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

        var users = await dbContext.Users
                .Where(u => u.EmailConfirmed) // Only email users who have confirmed their address
                .ToListAsync(cancellationToken);

        _logger.LogInformation("Found {UserCount} users to send recommendations to.", users.Count);

        foreach (var user in users)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation("Manual trigger was cancelled.");
                break;
            }

            var recommendations = await recommendationService.GetRecommendationsForUserAsync(user.Id, null, false, 1, 5);

            if (recommendations.Any())
            {
                var subject = "Your Weekly TV Show Recommendations!";
                var body = new StringBuilder();
                body.AppendLine($"Hi {user.DisplayName},");
                body.AppendLine("Here are some shows you might like based on your favorites:");
                body.AppendLine();
                foreach (var show in recommendations)
                {
                    body.AppendLine($"- {show.Title} ({show.ReleaseDate.Year})");
                }
                body.AppendLine();
                body.AppendLine("Happy watching!");

                await emailService.SendEmailAsync(user.Email!, user.DisplayName!, subject, body.ToString());
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Recommendation Email Job is stopping.");
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose() => _timer?.Dispose();
}
