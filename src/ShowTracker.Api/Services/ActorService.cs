using Microsoft.EntityFrameworkCore;
using ShowTracker.Api.Data;
using ShowTracker.Api.Dtos;
using ShowTracker.Api.Entities;
using ShowTracker.Api.Mappings;

namespace ShowTracker.Api.Services;

public interface IActorService
{
    Task<List<ActorSummaryDto>> GetAllActorsAsync(QueryParameters<ActorSortBy> parameters);
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

    public async Task<List<ActorSummaryDto>> GetAllActorsAsync(QueryParameters<ActorSortBy> parameters)
    {
        var actorsQuery = _dbContext.Actors.AsQueryable();

        actorsQuery = (parameters.SortBy, parameters.SortOrder) switch
        {
            (ActorSortBy.Name, SortOrder.desc) => actorsQuery.OrderByDescending(a => a.Name),
            (ActorSortBy.Name, SortOrder.asc) => actorsQuery.OrderBy(a => a.Name),
            _ => actorsQuery.OrderBy(a => a.Name)
        };

        var skip = (parameters.Page - 1) * parameters.PageSize;
        var actors = await actorsQuery
            .Include(a => a.Shows)
            .Skip(skip)
            .Take(parameters.PageSize)
            .ToListAsync();

        return actors.Select(actor => actor.ToSummaryDto()).ToList();
    }

    public async Task<ActorDetailsDto> GetActorByIdAsync(int id)
    {
        var actor = await _dbContext.Actors
            .Include(a => a.Shows)
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
