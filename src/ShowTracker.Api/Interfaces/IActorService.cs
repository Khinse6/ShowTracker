using ShowTracker.Api.Dtos;

namespace ShowTracker.Api.Interfaces;

public interface IActorService
{
    Task<List<ActorSummaryDto>> GetAllActorsAsync(QueryParameters<ActorSortBy> parameters);
    Task<ActorDetailsDto> GetActorByIdAsync(int id);
    Task<ActorSummaryDto> CreateActorAsync(CreateActorDto dto);
    Task UpdateActorAsync(int id, UpdateActorDto dto);
    Task DeleteActorAsync(int id);
}
