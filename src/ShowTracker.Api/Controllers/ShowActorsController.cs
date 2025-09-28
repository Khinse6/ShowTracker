using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShowTracker.Api.Dtos;
using ShowTracker.Api.Interfaces;

namespace ShowTracker.Api.Controllers;

[ApiController]
[Route("api/shows/{showId}/actors")]
[Authorize]
public class ShowActorsController : ExportableControllerBase
{
    private readonly IShowActorService _showActorService;

    public ShowActorsController(IShowActorService showActorService)
    {
        _showActorService = showActorService;
    }

    /// <summary>
    /// Gets all actors for a specific show.
    /// </summary>
    /// <param name="showId">The ID of the show.</param>
    /// <param name="parameters">Query parameters for sorting, pagination, and export format.</param>
    /// <response code="200">Returns the list of actors for the show.</response>
    /// <response code="404">If the show with the specified ID is not found.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponseDto<ActorSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetActorsForShow(
        int showId,
        [FromQuery] QueryParameters<ActorSortBy> parameters)
    {
        var paginatedActors = await _showActorService.GetActorsByShowIdAsync(showId, parameters);

        return parameters.Format == ExportFormat.json
            ? Ok(paginatedActors)
            : CreateExportOrOkResult(paginatedActors.Items, parameters.Format, $"Actors for Show ID: {showId}", $"show-{showId}-actors");
    }

    /// <summary>
    /// Adds an actor to a specific show. (Admin only))
    /// </summary>
    /// <param name="showId">The ID of the show.</param>
    /// <param name="actorId">The ID of the actor to add.</param>
    /// <response code="204">If the actor was successfully added to the show.</response>
    /// <response code="404">If the show or actor is not found.</response>
    [HttpPost("{actorId}")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddActorToShow(int showId, int actorId)
    {
        try
        {
            await _showActorService.AddActorToShowAsync(showId, actorId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Removes an actor from a specific show. (Admin only)
    /// </summary>
    /// <param name="showId">The ID of the show.</param>
    /// <param name="actorId">The ID of the actor to remove.</param>
    /// <response code="204">If the actor was successfully removed from the show.</response>
    /// <response code="404">If the show is not found.</response>
    [HttpDelete("{actorId}")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveActorFromShow(int showId, int actorId)
    {
        try
        {
            await _showActorService.RemoveActorFromShowAsync(showId, actorId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
