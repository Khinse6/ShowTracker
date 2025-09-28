using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using ShowTracker.Api.Data;
using ShowTracker.Api.Dtos;
using ShowTracker.Api.Entities;
using ShowTracker.Api.Mappings;
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

    public async Task<PaginatedResponseDto<ShowTypeDto>> GetAllAsync(QueryParameters<ShowTypeSortBy> parameters)
    {
        var cacheKey = $"showtypes-all-{parameters.GetCacheKey()}";
        var paginatedResponse = await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            var cts = GetOrCreateCancellationTokenSource();
            entry.AddExpirationToken(new CancellationChangeToken(cts.Token));
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);

            var query = _context.ShowTypes.AsNoTracking().AsQueryable();

            query = (parameters.SortBy, parameters.SortOrder) switch
            {
                (ShowTypeSortBy.Name, SortOrder.desc) => query.OrderByDescending(t => t.Name),
                _ => query.OrderBy(t => t.Name)
            };

            if (parameters.Format != ExportFormat.json)
            {
                return await query.ToExportResponseAsync(showTypes => showTypes.Select(st => st.ToDto()).ToList());
            }

            return await query.ToPaginatedDtoAsync(parameters,
                showTypes => showTypes.Select(st => st.ToDto()).ToList());
        });

        return paginatedResponse ?? new PaginatedResponseDto<ShowTypeDto>();
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
            return type.ToDto();
        });
    }

    public async Task<ShowTypeDto> CreateAsync(CreateShowTypeDto dto)
    {
        InvalidateCache();
        var entity = dto.ToEntity();
        _context.ShowTypes.Add(entity);
        await _context.SaveChangesAsync();
        return entity.ToDto();
    }

    public async Task<List<ShowTypeDto>> BulkCreateAsync(List<CreateShowTypeDto> dtos)
    {
        InvalidateCache();
        var entities = dtos.Select(d => d.ToEntity()).ToList();
        _context.ShowTypes.AddRange(entities);
        await _context.SaveChangesAsync();
        return entities.Select(e => e.ToDto()).ToList();
    }

    public async Task UpdateAsync(int id, UpdateShowTypeDto dto)
    {
        InvalidateCache();
        _cache.Remove($"showtype-{id}");
        var entity = await _context.ShowTypes.FindAsync(id);
        if (entity == null) { throw new KeyNotFoundException("Show type not found"); }
        dto.UpdateEntity(entity);
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
