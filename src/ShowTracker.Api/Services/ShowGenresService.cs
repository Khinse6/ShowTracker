using Microsoft.EntityFrameworkCore;
using ShowTracker.Api.Data;
using ShowTracker.Api.Dtos;

using ShowTracker.Api.Interfaces;
namespace ShowTracker.Api.Services;

public class ShowGenresService : IShowGenresService
{
    private readonly ShowStoreContext _dbContext;

    public ShowGenresService(ShowStoreContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<string>> GetGenresForShowAsync(int showId, QueryParameters<GenreSortBy> parameters)
    {
        var showExists = await _dbContext.Shows.AnyAsync(s => s.Id == showId);
        if (!showExists)
        {
            throw new KeyNotFoundException("Show not found");
        }

        var genresQuery = _dbContext.Genres
            .Where(g => g.Shows.Any(s => s.Id == showId));

        // Apply sorting using the QueryParameters
        genresQuery = (parameters.SortBy, parameters.SortOrder) switch
        {
            (GenreSortBy.Name, SortOrder.asc) => genresQuery.OrderBy(g => g.Name),
            (GenreSortBy.Name, SortOrder.desc) => genresQuery.OrderByDescending(g => g.Name),
            _ => genresQuery.OrderBy(g => g.Name) // Default sort
        };

        var skip = (parameters.Page - 1) * parameters.PageSize;
        return await genresQuery.Skip(skip).Take(parameters.PageSize)
            .Select(g => g.Name)
            .ToListAsync();
    }

    public async Task AddGenreToShowAsync(int showId, int genreId)
    {
        var show = await _dbContext.Shows
                .Include(s => s.Genres)
                .FirstOrDefaultAsync(s => s.Id == showId);

        var genre = await _dbContext.Genres.FindAsync(genreId);

        if (show == null)
        {
            throw new KeyNotFoundException("Show not found");
        }

        if (genre == null)
        {
            throw new KeyNotFoundException("Genre not found");
        }

        if (!show.Genres.Contains(genre))
        {
            show.Genres.Add(genre);
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task RemoveGenreFromShowAsync(int showId, int genreId)
    {
        var show = await _dbContext.Shows
                .Include(s => s.Genres)
                .FirstOrDefaultAsync(s => s.Id == showId);

        if (show == null)
        {
            throw new KeyNotFoundException("Show not found");
        }

        var genreToRemove = show.Genres.FirstOrDefault(g => g.Id == genreId);
        if (genreToRemove != null)
        {
            show.Genres.Remove(genreToRemove);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task ReplaceGenresForShowAsync(int showId, List<int> genreIds)
    {
        var show = await _dbContext.Shows
                .Include(s => s.Genres)
                .FirstOrDefaultAsync(s => s.Id == showId);

        if (show == null)
        {
            throw new KeyNotFoundException("Show not found");
        }

        var genres = await _dbContext.Genres
                .Where(g => genreIds.Contains(g.Id))
                .ToListAsync();

        // Validate that all requested genres were found
        if (genres.Count != genreIds.Distinct().Count())
        {
            throw new KeyNotFoundException("One or more genres not found.");
        }

        show.Genres.Clear();
        show.Genres.AddRange(genres);

        await _dbContext.SaveChangesAsync();
    }
}
