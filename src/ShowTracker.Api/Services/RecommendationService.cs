using Microsoft.EntityFrameworkCore;
using ShowTracker.Api.Data;
using ShowTracker.Api.Dtos;
using ShowTracker.Api.Mappings;
using System.Security.Claims;

namespace ShowTracker.Api.Services;

public interface IRecommendationService
{
    Task<List<ShowSummaryDto>> GetRecommendationsAsync();
}

public class RecommendationService : IRecommendationService
{
    private readonly ShowStoreContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RecommendationService(ShowStoreContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<List<ShowSummaryDto>> GetRecommendationsAsync()
    {
        var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

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
        var recommendations = await _dbContext.Shows
            .Include(s => s.Genres)
            .Include(s => s.ShowType)
            .Where(s => !favoriteShowIds.Contains(s.Id) && s.Genres.Any(g => favoriteGenres.Contains(g.Id)))
            .OrderByDescending(s => s.ReleaseDate) // Recommend newer shows first
            .Take(10) // Limit the number of recommendations
            .ToListAsync();

        return recommendations.Select(s => s.ToShowSummaryDto()).ToList();
    }
}
