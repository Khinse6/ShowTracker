using Microsoft.EntityFrameworkCore;
using ShowTracker.Api.Data;
using ShowTracker.Api.Dtos;
using ShowTracker.Api.Entities;

namespace ShowTracker.Api.Services;

public interface IShowTypeService
{
    Task<List<ShowTypeDto>> GetAllAsync();
    Task<ShowTypeDto?> GetByIdAsync(int id);
    Task<ShowTypeDto> CreateAsync(CreateShowTypeDto dto);
    Task<List<ShowTypeDto>> BulkCreateAsync(List<CreateShowTypeDto> dtos);
    Task UpdateAsync(int id, UpdateShowTypeDto dto);
    Task DeleteAsync(int id);
}

public class ShowTypeService : IShowTypeService
{
    private readonly ShowStoreContext _context;

    public ShowTypeService(ShowStoreContext context)
    {
        _context = context;
    }

    public async Task<List<ShowTypeDto>> GetAllAsync()
    {
        return await _context.ShowTypes
            .Select(t => new ShowTypeDto { Id = t.Id, Name = t.Name })
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<ShowTypeDto?> GetByIdAsync(int id)
    {
        var type = await _context.ShowTypes.FindAsync(id);
        if (type == null) { return null; }
        return new ShowTypeDto { Id = type.Id, Name = type.Name };
    }

    public async Task<ShowTypeDto> CreateAsync(CreateShowTypeDto dto)
    {
        var entity = new ShowType { Name = dto.Name };
        _context.ShowTypes.Add(entity);
        await _context.SaveChangesAsync();
        return new ShowTypeDto { Id = entity.Id, Name = entity.Name };
    }

    public async Task<List<ShowTypeDto>> BulkCreateAsync(List<CreateShowTypeDto> dtos)
    {
        var entities = dtos.Select(d => new ShowType { Name = d.Name }).ToList();
        _context.ShowTypes.AddRange(entities);
        await _context.SaveChangesAsync();
        return entities.Select(e => new ShowTypeDto { Id = e.Id, Name = e.Name }).ToList();
    }

    public async Task UpdateAsync(int id, UpdateShowTypeDto dto)
    {
        var entity = await _context.ShowTypes.FindAsync(id);
        if (entity == null) { throw new KeyNotFoundException("Show type not found"); }
        entity.Name = dto.Name;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.ShowTypes.FindAsync(id);
        if (entity == null) { throw new KeyNotFoundException("Show type not found"); }
        _context.ShowTypes.Remove(entity);
        await _context.SaveChangesAsync();
    }
}
