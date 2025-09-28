using Microsoft.EntityFrameworkCore;
using ShowTracker.Api.Data;
using ShowTracker.Api.Dtos;
using ShowTracker.Api.Mappings;
using ShowTracker.Api.Interfaces;


namespace ShowTracker.Api.Services;

public class RecommendationService : IRecommendationService
{
    private readonly ShowStoreContext _dbContext;

    public RecommendationService(ShowStoreContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<ShowSummaryDto>> GetRecommendationsForUserAsync(string userId, QueryParameters<ShowSortBy> parameters)
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

        // 3. Find other shows with those genres, calculate a recommendation score, and exclude already favorited shows.
        // The score is a simple sum of matching genres and the total number of times a show has been favorited.
        var recommendationsQuery = _dbContext.Shows
            .Include(s => s.Genres)
            .Include(s => s.ShowType)
            .Where(s => !favoriteShowIds.Contains(s.Id) && s.Genres.Any(g => favoriteGenres.Contains(g.Id)))
            .Select(s => new
            {
                Show = s,
                Score = s.Genres.Count(g => favoriteGenres.Contains(g.Id)) + s.FavoritedByUsers.Count()
            });

        // 4. Apply sorting. The primary sort is now our new recommendation score.
        // The user's requested sort is used as a secondary, tie-breaking sort.
        var orderedQuery = recommendationsQuery.OrderByDescending(x => x.Score);

        orderedQuery = (parameters.SortBy, parameters.SortOrder) switch
        {
            (ShowSortBy.Title, SortOrder.asc) => orderedQuery.ThenBy(x => x.Show.Title),
            (ShowSortBy.Title, SortOrder.desc) => orderedQuery.ThenByDescending(x => x.Show.Title),
            (ShowSortBy.ReleaseDate, SortOrder.asc) => orderedQuery.ThenBy(x => x.Show.ReleaseDate),
            _ => orderedQuery.ThenByDescending(x => x.Show.ReleaseDate) // Default secondary sort
        };

        // 5. Paginate and execute the query
        var skip = (parameters.Page - 1) * parameters.PageSize;
        var recommendations = await orderedQuery
            .Skip(skip)
            .Take(parameters.PageSize)
            .Select(x => x.Show) // Select only the Show entity for the final result
            .ToListAsync();

        return recommendations.Select(s => s.ToShowSummaryDto()).ToList();
    }
}
