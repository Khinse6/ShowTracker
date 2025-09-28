
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using ShowTracker.Api.Data;
using ShowTracker.Api.Dtos;
using ShowTracker.Api.Interfaces;
using ShowTracker.Api.Mappings;

namespace ShowTracker.Api.Services;

public class ActorService : IActorService
{
    private readonly ShowStoreContext _dbContext;
    private readonly IMemoryCache _cache;
    private const string ActorsTokenKey = "actors-cts";

    public ActorService(ShowStoreContext dbContext, IMemoryCache cache)
    {
        _dbContext = dbContext;
        _cache = cache;
    }

    public async Task<PaginatedResponseDto<ActorSummaryDto>> GetAllActorsAsync(QueryParameters<ActorSortBy> parameters)
    {
        var cacheKey = $"actors-all-{parameters.GetCacheKey()}";

        var paginatedResponse = await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            var cts = GetOrCreateCancellationTokenSource();
            entry.AddExpirationToken(new CancellationChangeToken(cts.Token));
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);

            var actorsQuery = _dbContext.Actors.AsNoTracking().AsQueryable();

            actorsQuery = (parameters.SortBy, parameters.SortOrder) switch
            {
                (ActorSortBy.Name, SortOrder.desc) => actorsQuery.OrderByDescending(a => a.Name),
                _ => actorsQuery.OrderBy(a => a.Name)
            };

            if (parameters.Format != ExportFormat.json)
            {
                return await actorsQuery.ToExportResponseAsync(actors => actors.Select(a => a.ToSummaryDto()).ToList());
            }

            return await actorsQuery.ToPaginatedDtoAsync(parameters, actors => actors.Select(actor => actor.ToSummaryDto()).ToList());
        });

        return paginatedResponse ?? new PaginatedResponseDto<ActorSummaryDto>();
    }

    public async Task<ActorDetailsDto> GetActorByIdAsync(int id)
    {
        var cacheKey = $"actor-{id}";

        var cachedActor = await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            var cts = GetOrCreateCancellationTokenSource();
            entry.AddExpirationToken(new CancellationChangeToken(cts.Token));
            entry.SlidingExpiration = TimeSpan.FromMinutes(30);

            var actor = await _dbContext.Actors
                .Include(a => a.Shows)
                .FirstOrDefaultAsync(a => a.Id == id);

            // Allow null to be cached to prevent repeated DB hits for non-existent IDs.
            return actor?.ToDetailsDto();
        });

        return cachedActor ?? throw new KeyNotFoundException("Actor not found");
    }

    public async Task<ActorSummaryDto> CreateActorAsync(CreateActorDto dto)
    {
        InvalidateCache();

        var actor = dto.ToEntity();
        _dbContext.Actors.Add(actor);
        await _dbContext.SaveChangesAsync();

        return actor.ToSummaryDto();
    }

    public async Task UpdateActorAsync(int id, UpdateActorDto dto)
    {
        InvalidateCache();
        _cache.Remove($"actor-{id}");

        var actor = await _dbContext.Actors.FindAsync(id);
        if (actor is null) { throw new KeyNotFoundException("Actor not found"); }

        dto.UpdateEntity(actor);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteActorAsync(int id)
    {
        InvalidateCache();
        _cache.Remove($"actor-{id}");

        var actor = await _dbContext.Actors.FindAsync(id);
        if (actor is null) { throw new KeyNotFoundException("Actor not found"); }

        _dbContext.Actors.Remove(actor);
        await _dbContext.SaveChangesAsync();
    }

    private CancellationTokenSource GetOrCreateCancellationTokenSource()
    {
        var cts = _cache.GetOrCreate(ActorsTokenKey, entry =>
        {
            entry.SetPriority(CacheItemPriority.NeverRemove);
            return new CancellationTokenSource();
        });

        return cts ?? throw new InvalidOperationException("Could not create or retrieve CancellationTokenSource from cache.");
    }

    private void InvalidateCache()
    {
        GetOrCreateCancellationTokenSource().Cancel();
        _cache.Remove(ActorsTokenKey);
    }
}
