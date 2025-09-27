using Microsoft.EntityFrameworkCore;
using ShowTracker.Api.Data;
using ShowTracker.Api.Dtos;
using ShowTracker.Api.Mappings;

namespace ShowTracker.Api.Services;

public interface IFavoritesService
{
    Task<List<ShowSummaryDto>> GetFavoritesAsync(string userId);
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

    public async Task<List<ShowSummaryDto>> GetFavoritesAsync(string userId)
    {
        var user = await _dbContext.Users
            .Include(u => u.FavoriteShows)
                .ThenInclude(s => s.Genres)
            .Include(u => u.FavoriteShows)
                .ThenInclude(s => s.ShowType) // ✅ Fix: also include ShowType
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        return user.FavoriteShows
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
