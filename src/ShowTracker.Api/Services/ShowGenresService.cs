using Microsoft.EntityFrameworkCore;
using ShowTracker.Api.Data;

namespace ShowTracker.Api.Services;

public interface IShowGenresService
{
    Task<List<string>> GetGenresForShowAsync(int showId, bool sortAsc, int page, int pageSize);
    Task AddGenreToShowAsync(int showId, int genreId);
    Task RemoveGenreFromShowAsync(int showId, int genreId);
    Task ReplaceGenresForShowAsync(int showId, List<int> genreIds);
}


public class ShowGenresService : IShowGenresService
{
    private readonly ShowStoreContext _dbContext;

    public ShowGenresService(ShowStoreContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<string>> GetGenresForShowAsync(int showId, bool sortAsc, int page, int pageSize)
    {
        var showExists = await _dbContext.Shows.AnyAsync(s => s.Id == showId);
        if (!showExists)
        {
            throw new KeyNotFoundException("Show not found");
        }

        var genresQuery = _dbContext.Genres
            .Where(g => g.Shows.Any(s => s.Id == showId));

        genresQuery = sortAsc ? genresQuery.OrderBy(g => g.Name) : genresQuery.OrderByDescending(g => g.Name);

        var skip = (page - 1) * pageSize;
        return await genresQuery.Skip(skip).Take(pageSize)
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

        var genre = await _dbContext.Genres.FindAsync(genreId);

        if (show == null)
        {
            throw new KeyNotFoundException("Show not found");
        }

        if (genre == null)
        {
            throw new KeyNotFoundException("Genre not found");
        }

        show.Genres.Remove(genre);
        await _dbContext.SaveChangesAsync();
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

        show.Genres.Clear();
        show.Genres.AddRange(genres);

        await _dbContext.SaveChangesAsync();
    }
}
