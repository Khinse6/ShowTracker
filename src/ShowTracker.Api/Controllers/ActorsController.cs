using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShowTracker.Api.Dtos;
using ShowTracker.Api.Interfaces;


namespace ShowTracker.Api.Controllers;

[ApiController]
[Route("api/actors")]
[Authorize]
public class ActorsController : ExportableControllerBase
{
    private readonly IActorService _actorService;

    public ActorsController(IActorService actorService)
    {
        _actorService = actorService;
    }

    /// <summary>
    /// Gets a list of all actors, with optional sorting and pagination. Can also export the list.
    /// </summary>
    /// <param name="parameters">Query parameters for sorting, pagination, and export format.</param>
    /// <response code="200">Returns a list of actors or a file if an export format is specified.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ActorSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] QueryParameters<ActorSortBy> parameters)
    {
        var actors = await _actorService.GetAllActorsAsync(parameters);

        return CreateExportOrOkResult(actors, parameters.Format, "Actors Report", "actors");
    }

    /// <summary>
    /// Gets a specific actor by their unique ID.
    /// </summary>
    /// <param name="id">The ID of the actor.</param>
    /// <response code="200">Returns the details of the actor.</response>
    /// <response code="404">If an actor with the specified ID is not found.</response>
    [HttpGet("{id}", Name = "GetActor")]
    public async Task<ActionResult<ActorDetailsDto>> GetById(int id)
    {
        try
        {
            var actor = await _actorService.GetActorByIdAsync(id);
            return Ok(actor);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Creates a new actor. (Admin only)
    /// </summary>
    /// <param name="dto">The data for the new actor.</param>
    /// <response code="201">Returns the newly created actor.</response>
    /// <response code="400">If the request body is invalid.</response>
    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ActorSummaryDto>> Create(CreateActorDto dto)
    {
        var actor = await _actorService.CreateActorAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = actor.Id }, actor);
    }

    /// <summary>
    /// Updates an existing actor. (Admin only)
    /// </summary>
    /// <param name="id">The ID of the actor to update.</param>
    /// <param name="dto">The updated data for the actor.</param>
    /// <response code="204">If the actor was updated successfully.</response>
    /// <response code="400">If the request body is invalid.</response>
    /// <response code="404">If an actor with the specified ID is not found.</response>
    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Update(int id, UpdateActorDto dto)
    {
        try
        {
            await _actorService.UpdateActorAsync(id, dto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Deletes an actor. (Admin only)
    /// </summary>
    /// <param name="id">The ID of the actor to delete.</param>
    /// <response code="204">If the actor was deleted successfully.</response>
    /// <response code="404">If an actor with the specified ID is not found.</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _actorService.DeleteActorAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
