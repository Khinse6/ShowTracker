using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShowTracker.Api.Dtos;
using ShowTracker.Api.Interfaces;

namespace ShowTracker.Api.Controllers;

[ApiController]
[Route("api/shows/{showId}/seasons")]
[Authorize]
public class SeasonsController : ExportableControllerBase
{
    private readonly IShowSeasonsService _seasonService;

    public SeasonsController(IShowSeasonsService seasonService)
    {
        _seasonService = seasonService;
    }

    /// <summary>
    /// Gets a list of all seasons for a specific show.
    /// </summary>
    /// <param name="showId">The ID of the show.</param>
    /// <param name="parameters">Query parameters for sorting, pagination, and export format.</param>
    /// <response code="200">Returns a list of seasons or a file if an export format is specified.</response>
    /// <response code="404">If the show with the specified ID is not found.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SeasonDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSeasons(
        int showId,
        [FromQuery] QueryParameters<SeasonSortBy> parameters)
    {
        var seasons = await _seasonService.GetSeasonsForShowAsync(showId, parameters);

        return CreateExportOrOkResult(seasons, parameters.Format, $"Seasons for Show ID: {showId}", $"show-{showId}-seasons");
    }

    /// <summary>
    /// Gets a specific season for a show.
    /// </summary>
    /// <param name="showId">The ID of the show.</param>
    /// <param name="seasonId">The ID of the season.</param>
    /// <response code="200">Returns the details of the season.</response>
    /// <response code="404">If the show or season is not found.</response>
    [HttpGet("{seasonId}", Name = "GetSeason")]
    public async Task<ActionResult<SeasonDto>> GetSeason(int showId, int seasonId)
    {
        var season = await _seasonService.GetSeasonAsync(showId, seasonId);
        if (season == null) { return NotFound(); }
        return Ok(season);
    }

    /// <summary>
    /// Creates a new season for a show. (Admin only)
    /// </summary>
    /// <param name="showId">The ID of the show.</param>
    /// <param name="dto">The data for the new season.</param>
    /// <response code="201">Returns the newly created season.</response>
    /// <response code="400">If the request body is invalid.</response>
    /// <response code="404">If the show is not found.</response>
    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<SeasonDto>> CreateSeason(int showId, [FromBody] CreateSeasonDto dto)
    {
        dto.ShowId = showId; // ensure the season is linked to the correct show
        var season = await _seasonService.CreateSeasonAsync(dto);
        return CreatedAtAction(nameof(GetSeason), new { showId, seasonId = season.Id }, season);
    }

    /// <summary>
    /// Creates multiple seasons for a show in a single request. (Admin only)
    /// </summary>
    /// <param name="showId">The ID of the show.</param>
    /// <param name="dtos">A list of seasons to create.</param>
    /// <response code="200">Returns the list of newly created seasons.</response>
    /// <response code="400">If the request body is empty or invalid.</response>
    /// <response code="404">If the show is not found.</response>
    [HttpPost("bulk")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<List<SeasonDto>>> CreateSeasonsBulk(int showId, [FromBody] List<CreateSeasonDto> dtos)
    {
        var seasons = await _seasonService.CreateSeasonsAsync(showId, dtos);
        return Ok(seasons);
    }

    /// <summary>
    /// Updates an existing season. (Admin only)
    /// </summary>
    /// <param name="showId">The ID of the show.</param>
    /// <param name="seasonId">The ID of the season to update.</param>
    /// <param name="dto">The updated data for the season.</param>
    /// <response code="204">If the season was updated successfully.</response>
    /// <response code="404">If the show or season is not found.</response>
    [HttpPut("{seasonId}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> UpdateSeason(int showId, int seasonId, [FromBody] UpdateSeasonDto dto)
    {
        try
        {
            await _seasonService.UpdateSeasonAsync(showId, seasonId, dto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Deletes a season from a show. (Admin only)
    /// </summary>
    /// <param name="showId">The ID of the show.</param>
    /// <param name="seasonId">The ID of the season to delete.</param>
    /// <response code="204">If the season was deleted successfully.</response>
    /// <response code="404">If the show or season is not found.</response>
    [HttpDelete("{seasonId}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteSeason(int showId, int seasonId)
    {
        try
        {
            await _seasonService.DeleteSeasonAsync(showId, seasonId);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
