using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using ShowTracker.Api.Data;
using ShowTracker.Api.Interfaces;
using ShowTracker.Api.Dtos;
using ShowTracker.Api.Mappings;

namespace ShowTracker.Api.Services;

public class ShowActorService : IShowActorService
{
    private readonly ShowStoreContext _context;
    private readonly IMemoryCache _cache;

    public ShowActorService(ShowStoreContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    private string ShowActorsTokenKey(int showId) => $"show-actors-cts-{showId}";

    public async Task<PaginatedResponseDto<ActorSummaryDto>> GetActorsByShowIdAsync(int showId, QueryParameters<ActorSortBy> parameters)
    {
        var cacheKey = $"show-actors-{showId}-{parameters.GetCacheKey()}";
        var paginatedResponse = await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            var cts = GetOrCreateCancellationTokenSource(showId);
            entry.AddExpirationToken(new CancellationChangeToken(cts.Token));
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);

            var actorsQuery = _context.Actors
                .AsNoTracking()
                .Where(a => a.Shows.Any(s => s.Id == showId));

            actorsQuery = (parameters.SortBy, parameters.SortOrder) switch
            {
                (ActorSortBy.Name, SortOrder.desc) => actorsQuery.OrderByDescending(a => a.Name),
                _ => actorsQuery.OrderBy(a => a.Name)
            };

            if (parameters.Format != ExportFormat.json)
            {
                return await actorsQuery.ToExportResponseAsync(actors => actors.Select(a => a.ToSummaryDto()).ToList());
            }

            return await actorsQuery.ToPaginatedDtoAsync(parameters, actors => actors.Select(a => a.ToSummaryDto()).ToList());
        });

        return paginatedResponse ?? new PaginatedResponseDto<ActorSummaryDto>();
    }

    public async Task AddActorToShowAsync(int showId, int actorId)
    {
        InvalidateCache(showId);
        var show = await _context.Shows
            .Include(s => s.Actors)
            .FirstOrDefaultAsync(s => s.Id == showId);

        if (show == null)
        {
            throw new KeyNotFoundException("Show not found.");
        }

        var actor = await _context.Actors.FindAsync(actorId);
        if (actor == null)
        {
            throw new KeyNotFoundException("Actor not found.");
        }

        if (!show.Actors.Any(a => a.Id == actorId))
        {
            show.Actors.Add(actor);
            await _context.SaveChangesAsync();
        }
    }

    public async Task RemoveActorFromShowAsync(int showId, int actorId)
    {
        InvalidateCache(showId);
        var show = await _context.Shows
            .Include(s => s.Actors)
            .FirstOrDefaultAsync(s => s.Id == showId);

        if (show == null)
        {
            throw new KeyNotFoundException("Show not found.");
        }

        var actor = show.Actors.FirstOrDefault(a => a.Id == actorId);
        if (actor != null)
        {
            show.Actors.Remove(actor);
            await _context.SaveChangesAsync();
        }
    }

    private CancellationTokenSource GetOrCreateCancellationTokenSource(int showId)
    {
        var cts = _cache.GetOrCreate(ShowActorsTokenKey(showId), entry =>
        {
            entry.SetPriority(CacheItemPriority.NeverRemove);
            return new CancellationTokenSource();
        });

        return cts ?? throw new InvalidOperationException("Could not create or retrieve CancellationTokenSource from cache.");
    }

    private void InvalidateCache(int showId)
    {
        GetOrCreateCancellationTokenSource(showId).Cancel();
        _cache.Remove(ShowActorsTokenKey(showId));
    }
}
