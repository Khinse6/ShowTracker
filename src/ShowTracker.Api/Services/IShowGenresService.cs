namespace ShowTracker.Api.Services;

public interface IShowGenresService
{
    Task<List<string>> GetGenresForShowAsync(int showId);
    Task AddGenreToShowAsync(int showId, int genreId);
    Task RemoveGenreFromShowAsync(int showId, int genreId);
    Task ReplaceGenresForShowAsync(int showId, List<int> genreIds);
}
