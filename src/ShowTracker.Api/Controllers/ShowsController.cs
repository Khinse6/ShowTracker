using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShowTracker.Api.Dtos;
using ShowTracker.Api.Interfaces;


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

    /// <summary>
    /// Gets a paginated list of all shows, with optional filtering and sorting. Can also export the list.
    /// </summary>
    /// <param name="genre">Filter shows by genre name.</param>
    /// <param name="type">Filter shows by type name.</param>
    /// <param name="searchTerm">Search for shows by title or description.</param>
    /// <param name="parameters">Query parameters for sorting, pagination, and export format.</param>
    /// <response code="200">Returns a list of shows or a file if an export format is specified.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponseDto<ShowSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllShows(
        [FromQuery] string? genre,
        [FromQuery] string? type,
        [FromQuery] string? searchTerm,
        [FromQuery] QueryParameters<ShowSortBy> parameters)
    {
        var paginatedShows = await _showService.GetAllShowsAsync(genre, type, searchTerm, parameters);

        return parameters.Format == ExportFormat.json
            ? Ok(paginatedShows)
            : CreateExportOrOkResult(paginatedShows.Items, parameters.Format, "Shows Report", "shows");
    }


    /// <summary>
    /// Gets a specific show by its unique ID.
    /// </summary>
    /// <param name="id">The ID of the show.</param>
    /// <response code="200">Returns the details of the show.</response>
    /// <response code="404">If a show with the specified ID is not found.</response>
    [HttpGet("{id}", Name = "GetShow")]
    public async Task<ActionResult<ShowDetailsDto>> GetShow(int id)
    {
        var show = await _showService.GetShowByIdAsync(id);
        return show is null ? NotFound() : Ok(show);
    }

    /// <summary>
    /// Creates a new show. (Admin only)
    /// </summary>
    /// <param name="dto">The data for the new show.</param>
    /// <response code="201">Returns the newly created show.</response>
    /// <response code="400">If the request body is invalid.</response>
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

    /// <summary>
    /// Updates an existing show. (Admin only)
    /// </summary>
    /// <param name="id">The ID of the show to update.</param>
    /// <param name="dto">The updated data for the show.</param>
    /// <response code="204">If the show was updated successfully.</response>
    /// <response code="400">If the request body is invalid.</response>
    /// <response code="404">If a show with the specified ID is not found.</response>
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

    /// <summary>
    /// Creates multiple shows in a single request. (Admin only)
    /// </summary>
    /// <param name="dtos">A list of shows to create.</param>
    /// <response code="200">Returns the list of newly created shows.</response>
    /// <response code="400">If the request body is empty or invalid.</response>
    [HttpPost("bulk")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<List<ShowSummaryDto>>> CreateShowsBulk([FromBody] List<CreateShowDto> dtos)
    {
        if (dtos == null || !dtos.Any()) { return BadRequest("No shows provided for creation."); }

        var createdShows = await _showService.CreateShowsAsync(dtos);
        return Ok(createdShows);
    }


    /// <summary>
    /// Deletes a show. (Admin only)
    /// </summary>
    /// <param name="id">The ID of the show to delete.</param>
    /// <response code="204">If the show was deleted successfully.</response>
    /// <response code="404">If a show with the specified ID is not found.</response>
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
