using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShowTracker.Api.Dtos;
using ShowTracker.Api.Helpers;
using ShowTracker.Api.Services;

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

    // GET api/shows/{showId}/seasons
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

    // GET api/shows/{showId}/seasons/{seasonId}
    [HttpGet("{seasonId}")]
    public async Task<ActionResult<SeasonDto>> GetSeason(int showId, int seasonId)
    {
        var season = await _seasonService.GetSeasonAsync(showId, seasonId);
        if (season == null) { return NotFound(); }
        return Ok(season);
    }

    // POST api/shows/{showId}/seasons
    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<SeasonDto>> CreateSeason(int showId, [FromBody] CreateSeasonDto dto)
    {
        dto.ShowId = showId; // ensure the season is linked to the correct show
        var season = await _seasonService.CreateSeasonAsync(dto);
        return CreatedAtAction(nameof(GetSeason), new { showId, seasonId = season.Id }, season);
    }

    [HttpPost("bulk")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<List<SeasonDto>>> CreateSeasonsBulk(int showId, [FromBody] List<CreateSeasonDto> dtos)
    {
        var seasons = await _seasonService.CreateSeasonsAsync(showId, dtos);
        return Ok(seasons);
    }

    // PUT api/shows/{showId}/seasons/{seasonId}
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

    // DELETE api/shows/{showId}/seasons/{seasonId}
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
