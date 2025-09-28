using ShowTracker.Api.Dtos;

namespace ShowTracker.Api.Interfaces;

public interface IFavoritesService
{
    Task<PaginatedResponseDto<ShowSummaryDto>> GetFavoritesAsync(string userId, QueryParameters<ShowSortBy> parameters);
    Task AddFavoriteAsync(string userId, int showId);
    Task RemoveFavoriteAsync(string userId, int showId);
}
