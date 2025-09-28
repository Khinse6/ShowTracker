using ShowTracker.Api.Dtos;

namespace ShowTracker.Api.Interfaces;

/// <summary>
/// Service for managing the relationship between shows and actors.
/// </summary>
public interface IShowActorService
{
    Task<IEnumerable<ActorSummaryDto>> GetActorsByShowIdAsync(int showId);
    Task AddActorToShowAsync(int showId, int actorId);
    Task RemoveActorFromShowAsync(int showId, int actorId);
}
