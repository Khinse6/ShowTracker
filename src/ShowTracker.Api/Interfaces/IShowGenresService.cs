using ShowTracker.Api.Dtos;

namespace ShowTracker.Api.Interfaces;

public interface IShowGenresService
{
    Task<List<string>> GetGenresForShowAsync(int showId, QueryParameters<GenreSortBy> parameters);
    Task AddGenreToShowAsync(int showId, int genreId);
    Task RemoveGenreFromShowAsync(int showId, int genreId);
    Task ReplaceGenresForShowAsync(int showId, List<int> genreIds);
}
