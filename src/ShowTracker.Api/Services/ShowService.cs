#nullable enable

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using ShowTracker.Api.Data;
using ShowTracker.Api.Dtos;
using ShowTracker.Api.Mappings;
using ShowTracker.Api.Interfaces;

namespace ShowTracker.Api.Services;

public class ShowService : IShowService
{
    private readonly ShowStoreContext _dbContext;
    private readonly IMemoryCache _cache;
    private const string ShowsTokenKey = "shows-cts";

    public ShowService(ShowStoreContext dbContext, IMemoryCache cache)
    {
        _dbContext = dbContext;
        _cache = cache;
    }

    public async Task<PaginatedResponseDto<ShowSummaryDto>> GetAllShowsAsync(
        string? genre,
        string? type,
        QueryParameters<ShowSortBy> parameters)
    {
        // A unique cache key is generated based on all query parameters.
        var cacheKey = $"shows-all-{genre}-{type}-{parameters.GetCacheKey()}";

        var paginatedResponse = await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            var cts = GetOrCreateCancellationTokenSource();
            entry.AddExpirationToken(new CancellationChangeToken(cts.Token));
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);

            var showsQuery = _dbContext.Shows
                .Include(s => s.Genres)
                .Include(s => s.ShowType)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(genre))
            { showsQuery = showsQuery.Where(s => s.Genres.Any(g => g.Name.ToLower() == genre.ToLower())); }

            if (!string.IsNullOrWhiteSpace(type))
            { showsQuery = showsQuery.Where(s => s.ShowType.Name.ToLower() == type.ToLower()); }

            showsQuery = (parameters.SortBy, parameters.SortOrder) switch
            {
                (ShowSortBy.Title, SortOrder.asc) => showsQuery.OrderBy(s => s.Title),
                (ShowSortBy.Title, SortOrder.desc) => showsQuery.OrderByDescending(s => s.Title),
                (ShowSortBy.ReleaseDate, SortOrder.asc) => showsQuery.OrderBy(s => s.ReleaseDate),
                _ => showsQuery.OrderByDescending(s => s.ReleaseDate)
            };

            if (parameters.Format != ExportFormat.json)
            {
                return await showsQuery.ToExportResponseAsync(shows => shows.Select(s => s.ToShowSummaryDto()).ToList());
            }

            // For JSON format, use the pagination extension method as before.
            return await showsQuery.ToPaginatedDtoAsync(parameters,
                shows => shows.Select(s => s.ToShowSummaryDto()).ToList());
        });

        return paginatedResponse ?? new PaginatedResponseDto<ShowSummaryDto>();
    }


    public async Task<ShowDetailsDto?> GetShowByIdAsync(int id)
    {
        var cacheKey = $"show-{id}";

        return await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            var cts = GetOrCreateCancellationTokenSource();
            entry.AddExpirationToken(new CancellationChangeToken(cts.Token));
            entry.SlidingExpiration = TimeSpan.FromMinutes(30);

            var show = await _dbContext.Shows
                    .Include(s => s.Genres)
                    .Include(s => s.ShowType)
                    .Include(s => s.Seasons)
                            .ThenInclude(se => se.Episodes)
                    .Include(s => s.Actors)
                    .AsNoTracking() // Use AsNoTracking for read-only queries
                    .FirstOrDefaultAsync(s => s.Id == id);

            return show?.ToShowDetailsDto();
        });
    }

    public async Task<ShowSummaryDto> CreateShowAsync(CreateShowDto newShowDto)
    {
        InvalidateCache();
        var show = newShowDto.ToEntity();
        _dbContext.Shows.Add(show);
        await _dbContext.SaveChangesAsync();
        return show.ToShowSummaryDto();
    }

    public async Task<List<ShowSummaryDto>> CreateShowsAsync(List<CreateShowDto> dtos)
    {
        if (dtos == null || !dtos.Any()) { return new List<ShowSummaryDto>(); }
        InvalidateCache();

        var shows = dtos.Select(dto => dto.ToEntity()).ToList();

        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            _dbContext.Shows.AddRange(shows);
            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }

        return shows.Select(s => s.ToShowSummaryDto()).ToList();
    }

    public async Task UpdateShowAsync(int id, UpdateShowDto updateShowDto)
    {
        InvalidateCache();
        _cache.Remove($"show-{id}");

        var existing = await _dbContext.Shows.FindAsync(id);
        if (existing is null)
        {
            throw new KeyNotFoundException("Show not found");
        }

        updateShowDto.UpdateEntity(existing);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteShowAsync(int id)
    {
        InvalidateCache();
        _cache.Remove($"show-{id}");

        var show = await _dbContext.Shows.FindAsync(id);
        if (show is null)
        {
            throw new KeyNotFoundException("Show not found");
        }

        _dbContext.Shows.Remove(show);
        await _dbContext.SaveChangesAsync();
    }

    private CancellationTokenSource GetOrCreateCancellationTokenSource()
    {
        var cts = _cache.GetOrCreate(ShowsTokenKey, entry =>
        {
            entry.SetPriority(CacheItemPriority.NeverRemove);
            return new CancellationTokenSource();
        });

        return cts ?? throw new InvalidOperationException("Could not create or retrieve CancellationTokenSource from cache.");
    }

    private void InvalidateCache()
    {
        GetOrCreateCancellationTokenSource().Cancel();
        _cache.Remove(ShowsTokenKey);
    }
}
