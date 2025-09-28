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

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ActorSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] QueryParameters<ActorSortBy> parameters)
    {
        var actors = await _actorService.GetAllActorsAsync(parameters);

        return CreateExportOrOkResult(actors, parameters.Format, "Actors Report", "actors");
    }

    [HttpGet("{id}")]
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

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ActorSummaryDto>> Create(CreateActorDto dto)
    {
        var actor = await _actorService.CreateActorAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = actor.Id }, actor);
    }

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
