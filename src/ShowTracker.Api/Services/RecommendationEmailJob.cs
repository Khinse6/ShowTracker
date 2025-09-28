using ShowTracker.Api.Interfaces;

namespace ShowTracker.Api.Services;

public class RecommendationEmailJob : IHostedService, IDisposable
{
    private readonly ILogger<RecommendationEmailJob> _logger;
    private Timer? _timer;
    private readonly IServiceProvider _serviceProvider;

    // We inject the IServiceProvider to create a new scope within the timer callback.
    // This is the correct pattern for resolving scoped services in a singleton background service.
    public RecommendationEmailJob(ILogger<RecommendationEmailJob> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
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
            // Create a new scope to resolve the job service.
            using var scope = _serviceProvider.CreateScope();
            var jobService = scope.ServiceProvider.GetRequiredService<IRecommendationJobService>();
            await jobService.TriggerManually(CancellationToken.None);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred in the recommendation email job's timer.");
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
