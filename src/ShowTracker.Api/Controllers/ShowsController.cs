using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShowTracker.Api.Dtos;
using ShowTracker.Api.Services;


namespace ShowTracker.Api.Controllers;

[ApiController]
[Route("api/shows")]
[Authorize]
public class ShowsController : ExportableControllerBase
{
    private readonly IShowService _showService;

    public ShowsController(IShowService showService)
    {
        _showService = showService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ShowSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllShows(
        [FromQuery] string? genre,
        [FromQuery] string? type,
        [FromQuery] QueryParameters<ShowSortBy> parameters)
    {
        var shows = await _showService.GetAllShowsAsync(genre, type, parameters);

        return CreateExportOrOkResult(shows, parameters.Format, "Shows Report", "shows");
    }


    [HttpGet("{id}", Name = "GetShow")]
    public async Task<ActionResult<ShowDetailsDto>> GetShow(int id)
    {
        var show = await _showService.GetShowByIdAsync(id);
        return show is null ? NotFound() : Ok(show);
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ShowSummaryDto>> CreateShow([FromBody] CreateShowDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
        {
            return BadRequest("Title is required.");
        }
        if (string.IsNullOrWhiteSpace(dto.Description))
        {
            return BadRequest("Description is required.");
        }

        var created = await _showService.CreateShowAsync(dto);
        return CreatedAtRoute("GetShow", new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> UpdateShow(int id, [FromBody] UpdateShowDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
        {
            return BadRequest("Title is required.");
        }
        if (string.IsNullOrWhiteSpace(dto.Description))
        {
            return BadRequest("Description is required.");
        }

        try
        {
            await _showService.UpdateShowAsync(id, dto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost("bulk")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<List<ShowSummaryDto>>> CreateShowsBulk([FromBody] List<CreateShowDto> dtos)
    {
        if (dtos == null || !dtos.Any()) { return BadRequest("No shows provided for creation."); }

        var createdShows = await _showService.CreateShowsAsync(dtos);
        return Ok(createdShows);
    }


    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteShow(int id)
    {
        try
        {
            await _showService.DeleteShowAsync(id);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        return NoContent();
    }
}
