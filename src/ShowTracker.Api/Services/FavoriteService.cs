using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using ShowTracker.Api.Data;
using ShowTracker.Api.Dtos;
using ShowTracker.Api.Mappings;
using ShowTracker.Api.Interfaces;

namespace ShowTracker.Api.Services;

public class FavoritesService : IFavoritesService
{
    private readonly ShowStoreContext _dbContext;
    private readonly IMemoryCache _cache;

    public FavoritesService(ShowStoreContext dbContext, IMemoryCache cache)
    {
        _dbContext = dbContext;
        _cache = cache;
    }

    private string UserFavoritesTokenKey(string userId) => $"favorites-cts-{userId}";

    public async Task<List<ShowSummaryDto>> GetFavoritesAsync(string userId, QueryParameters<ShowSortBy> parameters)
    {
        var cacheKey = $"favorites-{userId}-{parameters.GetCacheKey()}";

        var cachedFavorites = await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            var cts = GetOrCreateCancellationTokenSource(userId);
            entry.AddExpirationToken(new CancellationChangeToken(cts.Token));
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

            var userExists = await _dbContext.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
            {
                throw new KeyNotFoundException("User not found");
            }

            var favoritesQuery = _dbContext.Shows
                .Include(s => s.Genres)
                .Include(s => s.ShowType)
                .Where(s => s.FavoritedByUsers.Any(u => u.Id == userId));

            favoritesQuery = (parameters.SortBy, parameters.SortOrder) switch
            {
                (ShowSortBy.Title, SortOrder.asc) => favoritesQuery.OrderBy(s => s.Title),
                (ShowSortBy.Title, SortOrder.desc) => favoritesQuery.OrderByDescending(s => s.Title),
                (ShowSortBy.ReleaseDate, SortOrder.asc) => favoritesQuery.OrderBy(s => s.ReleaseDate),
                (ShowSortBy.ReleaseDate, SortOrder.desc) => favoritesQuery.OrderByDescending(s => s.ReleaseDate),
                _ => favoritesQuery.OrderBy(s => s.Title)
            };

            var skip = (parameters.Page - 1) * parameters.PageSize;
            var favoriteShows = await favoritesQuery
                .Skip(skip)
                .Take(parameters.PageSize)
                .ToListAsync();

            return favoriteShows
                .Select(s => s.ToShowSummaryDto())
                .ToList();
        });

        return cachedFavorites ?? new List<ShowSummaryDto>();
    }

    public async Task AddFavoriteAsync(string userId, int showId)
    {
        InvalidateCache(userId);

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
        InvalidateCache(userId);

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

    private CancellationTokenSource GetOrCreateCancellationTokenSource(string userId)
    {
        var cts = _cache.GetOrCreate(UserFavoritesTokenKey(userId), entry =>
        {
            entry.SetPriority(CacheItemPriority.NeverRemove);
            return new CancellationTokenSource();
        });

        return cts ?? throw new InvalidOperationException("Could not create or retrieve CancellationTokenSource from cache.");
    }

    private void InvalidateCache(string userId)
    {
        var tokenKey = UserFavoritesTokenKey(userId);
        GetOrCreateCancellationTokenSource(userId).Cancel();
        _cache.Remove(tokenKey);
    }
}
