using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShowTracker.Api.Dtos;
using ShowTracker.Api.Services;

namespace ShowTracker.Api.Controllers;

[ApiController]
[Route("api/actors")]
[Authorize]
public class ActorsController : ControllerBase
{
    private readonly IActorService _actorService;

    public ActorsController(IActorService actorService)
    {
        _actorService = actorService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ActorSummaryDto>>> GetAll(
        [FromQuery] string? sortBy,
        [FromQuery] bool descending = false,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var actors = await _actorService.GetAllActorsAsync(sortBy, descending, page, pageSize);
        return Ok(actors);
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
