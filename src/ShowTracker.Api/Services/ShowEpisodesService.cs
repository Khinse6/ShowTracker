using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using ShowTracker.Api.Data;
using ShowTracker.Api.Dtos;
using ShowTracker.Api.Entities;
using ShowTracker.Api.Interfaces;
using ShowTracker.Api.Mappings;

namespace ShowTracker.Api.Services;

public class ShowEpisodesService : IShowEpisodesService
{
    private readonly ShowStoreContext _context;
    private readonly IMemoryCache _cache;

    public ShowEpisodesService(ShowStoreContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    private string SeasonEpisodesTokenKey(int seasonId) => $"season-episodes-cts-{seasonId}";

    public async Task<PaginatedResponseDto<EpisodeDto>> GetEpisodesForSeasonAsync(int seasonId, QueryParameters<EpisodeSortBy> parameters)
    {
        var cacheKey = $"season-episodes-{seasonId}-{parameters.GetCacheKey()}";
        var paginatedResponse = await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            var cts = GetOrCreateCancellationTokenSource(seasonId);
            entry.AddExpirationToken(new CancellationChangeToken(cts.Token));
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);

            var episodesQuery = _context.Episodes
                .AsNoTracking()
                .Where(e => e.SeasonId == seasonId)
                .AsQueryable();

            episodesQuery = (parameters.SortBy, parameters.SortOrder) switch
            {
                (EpisodeSortBy.EpisodeNumber, SortOrder.asc) => episodesQuery.OrderBy(e => e.EpisodeNumber),
                (EpisodeSortBy.EpisodeNumber, SortOrder.desc) => episodesQuery.OrderByDescending(e => e.EpisodeNumber),
                (EpisodeSortBy.ReleaseDate, SortOrder.asc) => episodesQuery.OrderBy(e => e.ReleaseDate),
                (EpisodeSortBy.ReleaseDate, SortOrder.desc) => episodesQuery.OrderByDescending(e => e.ReleaseDate),
                _ => episodesQuery.OrderBy(e => e.EpisodeNumber)
            };

            if (parameters.Format != ExportFormat.json)
            {
                return await episodesQuery.ToExportResponseAsync(episodes => episodes.Select(e => e.ToDto()).ToList());
            }

            return await episodesQuery.ToPaginatedDtoAsync(parameters, episodes => episodes.Select(e => e.ToDto()).ToList());
        });

        return paginatedResponse ?? new PaginatedResponseDto<EpisodeDto>();
    }

    public async Task<EpisodeDto?> GetEpisodeAsync(int seasonId, int episodeId)
    {
        var cacheKey = $"season-{seasonId}-episode-{episodeId}";
        return await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            var cts = GetOrCreateCancellationTokenSource(seasonId);
            entry.AddExpirationToken(new CancellationChangeToken(cts.Token));
            entry.SlidingExpiration = TimeSpan.FromMinutes(30);
            var episode = await _context.Episodes
                .FirstOrDefaultAsync(e => e.Id == episodeId && e.SeasonId == seasonId);
            return episode?.ToDto();
        });
    }

    public async Task<EpisodeDto> CreateEpisodeAsync(int seasonId, CreateEpisodeDto dto)
    {
        InvalidateCache(seasonId);
        var season = await _context.Seasons.FindAsync(seasonId);
        if (season == null) { throw new KeyNotFoundException("Season not found"); }

        var episode = dto.ToEntity(); // This was already correct, but good to confirm.
        _context.Episodes.Add(episode);
        await _context.SaveChangesAsync();
        return episode.ToDto();
    }

    public async Task<List<EpisodeDto>> CreateEpisodesAsync(int seasonId, List<CreateEpisodeDto> dtos)
    {
        InvalidateCache(seasonId);
        var season = await _context.Seasons.FindAsync(seasonId);
        if (season == null) { throw new KeyNotFoundException("Season not found"); }

        var episodes = dtos.Select(dto => dto.ToEntity()).ToList();
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            _context.Episodes.AddRange(episodes);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }

        return episodes.Select(e => e.ToDto()).ToList();
    }


    public async Task UpdateEpisodeAsync(int seasonId, int episodeId, UpdateEpisodeDto dto)
    {
        InvalidateCache(seasonId);
        _cache.Remove($"season-{seasonId}-episode-{episodeId}");

        var episode = await _context.Episodes
            .FirstOrDefaultAsync(e => e.Id == episodeId && e.SeasonId == seasonId);

        if (episode == null) { throw new KeyNotFoundException("Episode not found"); }

        dto.UpdateEntity(episode);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteEpisodeAsync(int seasonId, int episodeId)
    {
        InvalidateCache(seasonId);
        _cache.Remove($"season-{seasonId}-episode-{episodeId}");

        var episode = await _context.Episodes
            .FirstOrDefaultAsync(e => e.Id == episodeId && e.SeasonId == seasonId);

        if (episode == null) { throw new KeyNotFoundException("Episode not found"); }

        _context.Episodes.Remove(episode);
        await _context.SaveChangesAsync();
    }

    private CancellationTokenSource GetOrCreateCancellationTokenSource(int seasonId)
    {
        var cts = _cache.GetOrCreate(SeasonEpisodesTokenKey(seasonId), entry =>
        {
            entry.SetPriority(CacheItemPriority.NeverRemove);
            return new CancellationTokenSource();
        });

        return cts ?? throw new InvalidOperationException("Could not create or retrieve CancellationTokenSource from cache.");
    }

    private void InvalidateCache(int seasonId)
    {
        var tokenKey = SeasonEpisodesTokenKey(seasonId);
        GetOrCreateCancellationTokenSource(seasonId).Cancel();
        _cache.Remove(tokenKey);
    }
}
