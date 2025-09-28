using Microsoft.EntityFrameworkCore;
using ShowTracker.Api.Data;
using ShowTracker.Api.Dtos;
using System.Text;
using ShowTracker.Api.Interfaces;

namespace ShowTracker.Api.Services;

public class RecommendationJobService : IRecommendationJobService
{
    private readonly ILogger<RecommendationJobService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public RecommendationJobService(ILogger<RecommendationJobService> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    public async Task TriggerManually(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Recommendation Email Job logic is running.");

        using var scope = _scopeFactory.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<ShowStoreContext>();
        var recommendationService = scope.ServiceProvider.GetRequiredService<IRecommendationService>();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

        var users = await dbContext.Users
                .Where(u => u.EmailConfirmed)
                .ToListAsync(cancellationToken);

        _logger.LogInformation("Found {UserCount} users to send recommendations to.", users.Count);

        foreach (var user in users)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation("Job was cancelled.");
                break;
            }

            var recommendations = await recommendationService.GetRecommendationsForUserAsync(user.Id, new QueryParameters<ShowSortBy> { Page = 1, PageSize = 5, SortBy = ShowSortBy.ReleaseDate, SortOrder = SortOrder.desc });

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
}
