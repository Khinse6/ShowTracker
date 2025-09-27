using Microsoft.EntityFrameworkCore;
using ShowTracker.Api.Data;
using ShowTracker.Api.Dtos;
using ShowTracker.Api.Entities;
using ShowTracker.Api.Mappings;

namespace ShowTracker.Api.Services;

public interface IActorService
{
    Task<List<ActorSummaryDto>> GetAllActorsAsync(string? sortBy, bool descending, int page, int pageSize);
    Task<ActorDetailsDto> GetActorByIdAsync(int id);
    Task<ActorSummaryDto> CreateActorAsync(CreateActorDto dto);
    Task UpdateActorAsync(int id, UpdateActorDto dto);
    Task DeleteActorAsync(int id);
}

public class ActorService : IActorService
{
    private readonly ShowStoreContext _dbContext;

    public ActorService(ShowStoreContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<ActorSummaryDto>> GetAllActorsAsync(string? sortBy, bool descending, int page, int pageSize)
    {
        var actorsQuery = _dbContext.Actors.AsQueryable();

        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            actorsQuery = sortBy.ToLower() switch
            {
                "name" => descending ? actorsQuery.OrderByDescending(a => a.Name) : actorsQuery.OrderBy(a => a.Name),
                _ => actorsQuery
            };
        }

        var skip = (page - 1) * pageSize;
        var actors = await actorsQuery
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync();

        return actors.Select(actor => actor.ToSummaryDto()).ToList();
    }

    public async Task<ActorDetailsDto> GetActorByIdAsync(int id)
    {
        var actor = await _dbContext.Actors
            .Include(a => a.Shows)
                .ThenInclude(s => s.ShowType)
            .Include(a => a.Shows)
                .ThenInclude(s => s.Genres)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (actor == null) { throw new KeyNotFoundException("Actor not found"); }

        return actor.ToDetailsDto();
    }

    public async Task<ActorSummaryDto> CreateActorAsync(CreateActorDto dto)
    {
        var actor = dto.ToEntity();
        _dbContext.Actors.Add(actor);
        await _dbContext.SaveChangesAsync();

        return actor.ToSummaryDto();
    }

    public async Task UpdateActorAsync(int id, UpdateActorDto dto)
    {
        var actor = await _dbContext.Actors.FindAsync(id);
        if (actor == null) { throw new KeyNotFoundException("Actor not found"); }

        dto.UpdateEntity(actor);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteActorAsync(int id)
    {
        var actor = await _dbContext.Actors.FindAsync(id);
        if (actor == null) { throw new KeyNotFoundException("Actor not found"); }

        _dbContext.Actors.Remove(actor);
        await _dbContext.SaveChangesAsync();
    }
}
