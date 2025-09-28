using Microsoft.EntityFrameworkCore;
using ShowTracker.Api.Data;
using ShowTracker.Api.Dtos;
using ShowTracker.Api.Mappings;

namespace ShowTracker.Api.Services;

public interface IFavoritesService
{
    Task<List<ShowSummaryDto>> GetFavoritesAsync(string userId, string? sortBy, bool sortAsc, int page, int pageSize);
    Task AddFavoriteAsync(string userId, int showId);
    Task RemoveFavoriteAsync(string userId, int showId);
}

public class FavoritesService : IFavoritesService
{
    private readonly ShowStoreContext _dbContext;

    public FavoritesService(ShowStoreContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<ShowSummaryDto>> GetFavoritesAsync(string userId, string? sortBy, bool sortAsc, int page, int pageSize)
    {
        var userExists = await _dbContext.Users.AnyAsync(u => u.Id == userId);
        if (!userExists)
        {
            throw new KeyNotFoundException("User not found");
        }

        // Build a query for shows that are favorited by the user
        var favoritesQuery = _dbContext.Shows
            .Include(s => s.Genres)
            .Include(s => s.ShowType)
            .Where(s => s.FavoritedByUsers.Any(u => u.Id == userId));

        // Apply sorting
        favoritesQuery = (sortBy?.ToLower(), sortAsc) switch
        {
            ("title", true) => favoritesQuery.OrderBy(s => s.Title),
            ("title", false) => favoritesQuery.OrderByDescending(s => s.Title),
            ("releasedate", true) => favoritesQuery.OrderBy(s => s.ReleaseDate),
            ("releasedate", false) => favoritesQuery.OrderByDescending(s => s.ReleaseDate),
            _ => favoritesQuery.OrderBy(s => s.Title) // Default sort for favorites
        };

        // Apply pagination and execute
        var skip = (page - 1) * pageSize;
        var favoriteShows = await favoritesQuery
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync();

        return favoriteShows
            .Select(s => s.ToShowSummaryDto())
            .ToList();
    }

    public async Task AddFavoriteAsync(string userId, int showId)
    {
        var user = await _dbContext.Users
            .Include(u => u.FavoriteShows)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        var show = await _dbContext.Shows.FindAsync(showId);
        if (show == null)
        {
            throw new KeyNotFoundException("Show not found");
        }

        if (!user.FavoriteShows.Any(s => s.Id == showId))
        {
            user.FavoriteShows.Add(show);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task RemoveFavoriteAsync(string userId, int showId)
    {
        var user = await _dbContext.Users
            .Include(u => u.FavoriteShows)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        var show = user.FavoriteShows.FirstOrDefault(s => s.Id == showId);
        if (show == null)
        {
            throw new KeyNotFoundException("Show not in favorites");
        }

        user.FavoriteShows.Remove(show);
        await _dbContext.SaveChangesAsync();
    }
}
