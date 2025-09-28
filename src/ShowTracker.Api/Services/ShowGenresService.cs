using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using ShowTracker.Api.Data;
using ShowTracker.Api.Dtos;
using ShowTracker.Api.Mappings;

using ShowTracker.Api.Interfaces;
namespace ShowTracker.Api.Services;

public class ShowGenresService : IShowGenresService
{
    private readonly ShowStoreContext _dbContext;
    private readonly IMemoryCache _cache;

    public ShowGenresService(ShowStoreContext dbContext, IMemoryCache cache)
    {
        _dbContext = dbContext;
        _cache = cache;
    }

    private string ShowGenresTokenKey(int showId) => $"show-genres-cts-{showId}";

    public async Task<PaginatedResponseDto<GenreDto>> GetGenresForShowAsync(int showId, QueryParameters<GenreSortBy> parameters)
    {
        var cacheKey = $"show-genres-{showId}-{parameters.GetCacheKey()}";

        var paginatedResponse = await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            var cts = GetOrCreateCancellationTokenSource(showId);
            entry.AddExpirationToken(new CancellationChangeToken(cts.Token));
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);

            var showExists = await _dbContext.Shows.AnyAsync(s => s.Id == showId);
            if (!showExists)
            {
                throw new KeyNotFoundException("Show not found");
            }

            var genresQuery = _dbContext.Genres
                .AsNoTracking()
                .Where(g => g.Shows.Any(s => s.Id == showId));

            genresQuery = (parameters.SortBy, parameters.SortOrder) switch
            {
                (GenreSortBy.Name, SortOrder.asc) => genresQuery.OrderBy(g => g.Name),
                (GenreSortBy.Name, SortOrder.desc) => genresQuery.OrderByDescending(g => g.Name),
                _ => genresQuery.OrderBy(g => g.Name)
            };

            if (parameters.Format != ExportFormat.json)
            {
                return await genresQuery.ToExportResponseAsync(genres => genres.Select(g => g.ToDto()).ToList());
            }

            return await genresQuery.ToPaginatedDtoAsync(parameters, genres => genres.Select(g => g.ToDto()).ToList());
        });

        return paginatedResponse ?? new PaginatedResponseDto<GenreDto>();
    }

    public async Task AddGenreToShowAsync(int showId, int genreId)
    {
        InvalidateCache(showId);
        var show = await _dbContext.Shows
                .Include(s => s.Genres)
                .FirstOrDefaultAsync(s => s.Id == showId);

        var genre = await _dbContext.Genres.FindAsync(genreId);

        if (show is null)
        {
            throw new KeyNotFoundException("Show not found");
        }

        if (genre is null)
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
        InvalidateCache(showId);
        var show = await _dbContext.Shows
                .Include(s => s.Genres)
                .FirstOrDefaultAsync(s => s.Id == showId);

        if (show is null)
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
        InvalidateCache(showId);
        var show = await _dbContext.Shows
                .Include(s => s.Genres)
                .FirstOrDefaultAsync(s => s.Id == showId);

        if (show is null)
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

    private CancellationTokenSource GetOrCreateCancellationTokenSource(int showId)
    {
        var cts = _cache.GetOrCreate(ShowGenresTokenKey(showId), entry =>
        {
            entry.SetPriority(CacheItemPriority.NeverRemove);
            return new CancellationTokenSource();
        });

        return cts ?? throw new InvalidOperationException("Could not create or retrieve CancellationTokenSource from cache.");
    }

    private void InvalidateCache(int showId)
    {
        var tokenKey = ShowGenresTokenKey(showId);
        GetOrCreateCancellationTokenSource(showId).Cancel();
        _cache.Remove(tokenKey);
    }
}
