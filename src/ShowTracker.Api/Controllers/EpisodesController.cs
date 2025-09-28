using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShowTracker.Api.Dtos;
using ShowTracker.Api.Helpers;
using ShowTracker.Api.Services;

namespace ShowTracker.Api.Controllers;

[ApiController]
[Route("api/seasons/{seasonId}/episodes")]
[Authorize]
public class EpisodesController : ExportableControllerBase
{
    private readonly IShowEpisodesService _episodeService;

    public EpisodesController(IShowEpisodesService episodeService)
    {
        _episodeService = episodeService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EpisodeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEpisodes(
        int seasonId,
        [FromQuery] QueryParameters<EpisodeSortBy> parameters)
    {
        var episodes = await _episodeService.GetEpisodesForSeasonAsync(seasonId, parameters);

        return CreateExportOrOkResult(episodes, parameters.Format, $"Episodes for Season ID: {seasonId}", $"season-{seasonId}-episodes");
    }

    [HttpGet("{episodeId}")]
    public async Task<ActionResult<EpisodeDto>> GetEpisode(int seasonId, int episodeId)
    {
        var episode = await _episodeService.GetEpisodeAsync(seasonId, episodeId);
        if (episode == null) { return NotFound(); }
        return Ok(episode);
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<EpisodeDto>> CreateEpisode(int seasonId, [FromBody] CreateEpisodeDto dto)
    {
        var episode = await _episodeService.CreateEpisodeAsync(seasonId, dto);
        return CreatedAtAction(nameof(GetEpisode), new { seasonId, episodeId = episode.Id }, episode);
    }

    [HttpPost("bulk")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<List<EpisodeDto>>> CreateEpisodesBulk(int seasonId, [FromBody] List<CreateEpisodeDto> dtos)
    {
        var episodes = await _episodeService.CreateEpisodesAsync(seasonId, dtos);
        return Ok(episodes);
    }


    [HttpPut("{episodeId}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> UpdateEpisode(int seasonId, int episodeId, [FromBody] UpdateEpisodeDto dto)
    {
        try
        {
            await _episodeService.UpdateEpisodeAsync(seasonId, episodeId, dto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{episodeId}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteEpisode(int seasonId, int episodeId)
    {
        try
        {
            await _episodeService.DeleteEpisodeAsync(seasonId, episodeId);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
