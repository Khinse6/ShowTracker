using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using ShowTracker.Api.Data;
using ShowTracker.Api.Dtos;
using ShowTracker.Api.Entities;
using ShowTracker.Api.Interfaces;

namespace ShowTracker.Api.Services;

public class ShowTypeService : IShowTypeService
{
    private readonly ShowStoreContext _context;
    private readonly IMemoryCache _cache;
    private const string ShowTypesTokenKey = "showtypes-cts";

    public ShowTypeService(ShowStoreContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<List<ShowTypeDto>> GetAllAsync(QueryParameters<ShowTypeSortBy> parameters)
    {
        var cacheKey = $"showtypes-all-{parameters.GetCacheKey()}";
        var cachedShowTypes = await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            var cts = GetOrCreateCancellationTokenSource();
            entry.AddExpirationToken(new CancellationChangeToken(cts.Token));
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);

            var query = _context.ShowTypes.AsQueryable();

            query = (parameters.SortBy, parameters.SortOrder) switch
            {
                (ShowTypeSortBy.Name, SortOrder.asc) => query.OrderBy(t => t.Name),
                (ShowTypeSortBy.Name, SortOrder.desc) => query.OrderByDescending(t => t.Name),
                _ => query.OrderBy(t => t.Name)
            };

            var skip = (parameters.Page - 1) * parameters.PageSize;
            return await query
                .Skip(skip)
                .Take(parameters.PageSize)
                .Select(t => new ShowTypeDto { Id = t.Id, Name = t.Name })
                .AsNoTracking()
                .ToListAsync();
        });

        return cachedShowTypes ?? new List<ShowTypeDto>();
    }

    public async Task<ShowTypeDto?> GetByIdAsync(int id)
    {
        var cacheKey = $"showtype-{id}";
        return await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            var cts = GetOrCreateCancellationTokenSource();
            entry.AddExpirationToken(new CancellationChangeToken(cts.Token));
            entry.SlidingExpiration = TimeSpan.FromHours(1);
            var type = await _context.ShowTypes.FindAsync(id);
            if (type == null) { return null; }
            return new ShowTypeDto { Id = type.Id, Name = type.Name };
        });
    }

    public async Task<ShowTypeDto> CreateAsync(CreateShowTypeDto dto)
    {
        InvalidateCache();
        var entity = new ShowType { Name = dto.Name };
        _context.ShowTypes.Add(entity);
        await _context.SaveChangesAsync();
        return new ShowTypeDto { Id = entity.Id, Name = entity.Name };
    }

    public async Task<List<ShowTypeDto>> BulkCreateAsync(List<CreateShowTypeDto> dtos)
    {
        InvalidateCache();
        var entities = dtos.Select(d => new ShowType { Name = d.Name }).ToList();
        _context.ShowTypes.AddRange(entities);
        await _context.SaveChangesAsync();
        return entities.Select(e => new ShowTypeDto { Id = e.Id, Name = e.Name }).ToList();
    }

    public async Task UpdateAsync(int id, UpdateShowTypeDto dto)
    {
        InvalidateCache();
        _cache.Remove($"showtype-{id}");
        var entity = await _context.ShowTypes.FindAsync(id);
        if (entity == null) { throw new KeyNotFoundException("Show type not found"); }
        entity.Name = dto.Name;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        InvalidateCache();
        _cache.Remove($"showtype-{id}");
        var entity = await _context.ShowTypes.FindAsync(id);
        if (entity == null) { throw new KeyNotFoundException("Show type not found"); }
        _context.ShowTypes.Remove(entity);
        await _context.SaveChangesAsync();
    }

    private CancellationTokenSource GetOrCreateCancellationTokenSource()
    {
        var cts = _cache.GetOrCreate(ShowTypesTokenKey, entry =>
        {
            entry.SetPriority(CacheItemPriority.NeverRemove);
            return new CancellationTokenSource();
        });

        return cts ?? throw new InvalidOperationException("Could not create or retrieve CancellationTokenSource from cache.");
    }

    private void InvalidateCache()
    {
        GetOrCreateCancellationTokenSource().Cancel();
        _cache.Remove(ShowTypesTokenKey);
    }
}
