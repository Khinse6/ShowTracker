using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using ShowTracker.Api.Data;
using ShowTracker.Api.Dtos;
using ShowTracker.Api.Entities;
using ShowTracker.Api.Interfaces;
using ShowTracker.Api.Mappings;

namespace ShowTracker.Api.Services;

public class ShowSeasonsService : IShowSeasonsService
{
    private readonly ShowStoreContext _context;
    private readonly IMemoryCache _cache;

    public ShowSeasonsService(ShowStoreContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    private string ShowSeasonsTokenKey(int showId) => $"show-seasons-cts-{showId}";

    public async Task<List<SeasonDto>> GetSeasonsForShowAsync(int showId, QueryParameters<SeasonSortBy> parameters)
    {
        var cacheKey = $"show-seasons-{showId}-{parameters.GetCacheKey()}";
        var cachedSeasons = await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            var cts = GetOrCreateCancellationTokenSource(showId);
            entry.AddExpirationToken(new CancellationChangeToken(cts.Token));
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);

            var seasonsQuery = _context.Seasons
                .Include(s => s.Episodes)
                .Where(s => s.ShowId == showId)
                .AsQueryable();

            seasonsQuery = (parameters.SortBy, parameters.SortOrder) switch
            {
                (SeasonSortBy.SeasonNumber, SortOrder.asc) => seasonsQuery.OrderBy(s => s.SeasonNumber),
                (SeasonSortBy.SeasonNumber, SortOrder.desc) => seasonsQuery.OrderByDescending(s => s.SeasonNumber),
                (SeasonSortBy.ReleaseDate, SortOrder.asc) => seasonsQuery.OrderBy(s => s.ReleaseDate),
                (SeasonSortBy.ReleaseDate, SortOrder.desc) => seasonsQuery.OrderByDescending(s => s.ReleaseDate),
                _ => seasonsQuery.OrderBy(s => s.SeasonNumber)
            };

            var skip = (parameters.Page - 1) * parameters.PageSize;
            var seasons = await seasonsQuery
                .Skip(skip)
                .Take(parameters.PageSize)
                .ToListAsync();

            return seasons.Select(s => s.ToDto()).ToList();
        });

        return cachedSeasons ?? new List<SeasonDto>();
    }


    public async Task<SeasonDto?> GetSeasonAsync(int showId, int seasonId)
    {
        var cacheKey = $"show-season-{showId}-{seasonId}";
        return await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            var cts = GetOrCreateCancellationTokenSource(showId);
            entry.AddExpirationToken(new CancellationChangeToken(cts.Token));
            entry.SlidingExpiration = TimeSpan.FromMinutes(30);
            var season = await _context.Seasons
                .Include(s => s.Episodes)
                .FirstOrDefaultAsync(s => s.Id == seasonId && s.ShowId == showId);
            return season?.ToDto();
        });
    }

    public async Task<SeasonDto> CreateSeasonAsync(CreateSeasonDto dto)
    {
        InvalidateCache(dto.ShowId);
        var season = new Season
        {
            ShowId = dto.ShowId,
            SeasonNumber = dto.SeasonNumber,
            ReleaseDate = dto.ReleaseDate
        };

        _context.Seasons.Add(season);
        await _context.SaveChangesAsync();

        return season.ToDto();
    }

    public async Task<List<SeasonDto>> CreateSeasonsAsync(int showId, List<CreateSeasonDto> dtos)
    {
        InvalidateCache(showId);
        var show = await _context.Shows.FindAsync(showId);
        if (show == null) { throw new KeyNotFoundException("Show not found"); }

        var seasons = dtos.Select(dto => new Season
        {
            ShowId = showId,
            SeasonNumber = dto.SeasonNumber,
            ReleaseDate = dto.ReleaseDate
        }).ToList();

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            _context.Seasons.AddRange(seasons);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }

        return seasons.Select(s => s.ToDto()).ToList();
    }

    public async Task UpdateSeasonAsync(int showId, int seasonId, UpdateSeasonDto dto)
    {
        InvalidateCache(showId);
        _cache.Remove($"show-season-{showId}-{seasonId}");

        var season = await _context.Seasons
            .FirstOrDefaultAsync(s => s.Id == seasonId && s.ShowId == showId);

        if (season == null) { throw new KeyNotFoundException("Season not found"); }

        season.SeasonNumber = dto.SeasonNumber;
        season.ReleaseDate = dto.ReleaseDate;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteSeasonAsync(int showId, int seasonId)
    {
        InvalidateCache(showId);
        _cache.Remove($"show-season-{showId}-{seasonId}");

        var season = await _context.Seasons
            .FirstOrDefaultAsync(s => s.Id == seasonId && s.ShowId == showId);

        if (season == null) { throw new KeyNotFoundException("Season not found"); }

        _context.Seasons.Remove(season);
        await _context.SaveChangesAsync();
    }

    private CancellationTokenSource GetOrCreateCancellationTokenSource(int showId)
    {
        var cts = _cache.GetOrCreate(ShowSeasonsTokenKey(showId), entry =>
        {
            entry.SetPriority(CacheItemPriority.NeverRemove);
            return new CancellationTokenSource();
        });

        return cts ?? throw new InvalidOperationException("Could not create or retrieve CancellationTokenSource from cache.");
    }

    private void InvalidateCache(int showId)
    {
        var tokenKey = ShowSeasonsTokenKey(showId);
        GetOrCreateCancellationTokenSource(showId).Cancel();
        _cache.Remove(tokenKey);
    }
}
