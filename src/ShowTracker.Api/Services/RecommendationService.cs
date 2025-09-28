using Microsoft.EntityFrameworkCore;
using ShowTracker.Api.Data;
using ShowTracker.Api.Dtos;
using ShowTracker.Api.Mappings;
using System.Security.Claims;

namespace ShowTracker.Api.Services;

public interface IRecommendationService
{
    Task<List<ShowSummaryDto>> GetRecommendationsForUserAsync(string userId, string? sortBy, bool sortAsc, int page, int pageSize);
}

public class RecommendationService : IRecommendationService
{
    private readonly ShowStoreContext _dbContext;

    public RecommendationService(ShowStoreContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<ShowSummaryDto>> GetRecommendationsForUserAsync(string userId, string? sortBy, bool sortAsc, int page, int pageSize)
    {
        if (string.IsNullOrEmpty(userId))
        {
            // This should not happen for an authorized endpoint, but it's good practice to check.
            return new List<ShowSummaryDto>();
        }

        // 1. Get the IDs of the user's favorite shows by querying the User entity
        var favoriteShowIds = await _dbContext.Users
            .Where(u => u.Id == userId)
            .SelectMany(u => u.FavoriteShows.Select(s => s.Id))
            .ToListAsync();

        if (!favoriteShowIds.Any())
        {
            return new List<ShowSummaryDto>(); // No favorites, no recommendations
        }

        // 2. Get the genres from those favorite shows
        var favoriteGenres = await _dbContext.Shows
            .Where(s => favoriteShowIds.Contains(s.Id))
            .SelectMany(s => s.Genres.Select(g => g.Id))
            .Distinct()
            .ToListAsync();

        // 3. Find other shows with those genres, excluding already favorited shows
        var recommendationsQuery = _dbContext.Shows
            .Include(s => s.Genres)
            .Include(s => s.ShowType)
            .Where(s => !favoriteShowIds.Contains(s.Id) && s.Genres.Any(g => favoriteGenres.Contains(g.Id)));

        // 4. Apply sorting
        // We use a switch statement for security and control over what can be sorted.
        recommendationsQuery = (sortBy?.ToLower(), sortAsc) switch
        {
            ("title", true) => recommendationsQuery.OrderBy(s => s.Title),
            ("title", false) => recommendationsQuery.OrderByDescending(s => s.Title),
            ("releasedate", true) => recommendationsQuery.OrderBy(s => s.ReleaseDate),
            ("releasedate", false) => recommendationsQuery.OrderByDescending(s => s.ReleaseDate),
            // Default sort
            _ => recommendationsQuery.OrderByDescending(s => s.ReleaseDate)
        };

        // 5. Paginate and execute the query
        var skip = (page - 1) * pageSize;
        var recommendations = await recommendationsQuery
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync();

        return recommendations.Select(s => s.ToShowSummaryDto()).ToList();
    }
}
