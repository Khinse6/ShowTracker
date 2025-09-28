using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ShowTracker.Api.Data;
using ShowTracker.Api.Dtos;
using ShowTracker.Api.Mappings;
using ShowTracker.Api.Interfaces;


namespace ShowTracker.Api.Services;

public class RecommendationService : IRecommendationService
{
    private readonly ShowStoreContext _dbContext;
    private readonly IMemoryCache _cache;

    public RecommendationService(ShowStoreContext dbContext, IMemoryCache cache)
    {
        _dbContext = dbContext;
        _cache = cache;
    }

    public async Task<List<ShowSummaryDto>> GetRecommendationsForUserAsync(string userId, QueryParameters<ShowSortBy> parameters)
    {
        var cacheKey = $"recommendations-{userId}-{parameters.GetCacheKey()}";

        var cachedRecommendations = await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            // Recommendations are expensive to generate, so cache for a reasonable time.
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);

            if (string.IsNullOrEmpty(userId))
            {
                return new List<ShowSummaryDto>();
            }

            var favoriteShowIds = await _dbContext.Users
                .Where(u => u.Id == userId)
                .SelectMany(u => u.FavoriteShows.Select(s => s.Id))
                .ToListAsync();

            if (!favoriteShowIds.Any())
            {
                return new List<ShowSummaryDto>();
            }

            var favoriteGenres = await _dbContext.Shows
                .Where(s => favoriteShowIds.Contains(s.Id))
                .SelectMany(s => s.Genres.Select(g => g.Id))
                .Distinct()
                .ToListAsync();

            var recommendationsQuery = _dbContext.Shows
                .Include(s => s.Genres)
                .Include(s => s.ShowType)
                .Where(s => !favoriteShowIds.Contains(s.Id) && s.Genres.Any(g => favoriteGenres.Contains(g.Id)))
                .Select(s => new
                {
                    Show = s,
                    Score = s.Genres.Count(g => favoriteGenres.Contains(g.Id)) + s.FavoritedByUsers.Count()
                });

            var orderedQuery = recommendationsQuery.OrderByDescending(x => x.Score);

            orderedQuery = (parameters.SortBy, parameters.SortOrder) switch
            {
                (ShowSortBy.Title, SortOrder.asc) => orderedQuery.ThenBy(x => x.Show.Title),
                (ShowSortBy.Title, SortOrder.desc) => orderedQuery.ThenByDescending(x => x.Show.Title),
                (ShowSortBy.ReleaseDate, SortOrder.asc) => orderedQuery.ThenBy(x => x.Show.ReleaseDate),
                _ => orderedQuery.ThenByDescending(x => x.Show.ReleaseDate)
            };

            var skip = (parameters.Page - 1) * parameters.PageSize;
            var recommendations = await orderedQuery
                .Skip(skip)
                .Take(parameters.PageSize)
                .Select(x => x.Show)
                .ToListAsync();

            return recommendations.Select(s => s.ToShowSummaryDto()).ToList();
        });

        return cachedRecommendations ?? new List<ShowSummaryDto>();
    }
}
