using ShowTracker.Api.Dtos;

namespace ShowTracker.Api.Interfaces;

public interface IRecommendationService
{
    Task<List<ShowSummaryDto>> GetRecommendationsForUserAsync(string userId, QueryParameters<ShowSortBy> parameters);
}
