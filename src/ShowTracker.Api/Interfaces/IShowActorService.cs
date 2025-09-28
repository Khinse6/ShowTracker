using ShowTracker.Api.Dtos;

namespace ShowTracker.Api.Interfaces;

/// <summary>
/// Service for managing the relationship between shows and actors.
/// </summary>
public interface IShowActorService
{
    Task<PaginatedResponseDto<ActorSummaryDto>> GetActorsByShowIdAsync(int showId, QueryParameters<ActorSortBy> parameters);
    Task AddActorToShowAsync(int showId, int actorId);
    Task RemoveActorFromShowAsync(int showId, int actorId);
}
