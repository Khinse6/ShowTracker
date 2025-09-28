using ShowTracker.Api.Dtos;

namespace ShowTracker.Api.Interfaces;

public interface IRecommendationService
{
    Task<PaginatedResponseDto<ShowSummaryDto>> GetRecommendationsForUserAsync(string userId, QueryParameters<ShowSortBy> parameters);
}
