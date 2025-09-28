using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShowTracker.Api.Dtos;
using ShowTracker.Api.Interfaces;

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

    /// <summary>
    /// Gets a list of all episodes for a specific season.
    /// </summary>
    /// <param name="seasonId">The ID of the season.</param>
    /// <param name="parameters">Query parameters for sorting, pagination, and export format.</param>
    /// <response code="200">Returns a list of episodes or a file if an export format is specified.</response>
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

    /// <summary>
    /// Gets a specific episode from a season.
    /// </summary>
    /// <param name="seasonId">The ID of the season.</param>
    /// <param name="episodeId">The ID of the episode.</param>
    /// <response code="200">Returns the details of the episode.</response>
    /// <response code="404">If the season or episode is not found.</response>
    [HttpGet("{episodeId}", Name = "GetEpisode")]
    [ProducesResponseType(typeof(EpisodeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EpisodeDto>> GetEpisode(int seasonId, int episodeId)
    {
        var episode = await _episodeService.GetEpisodeAsync(seasonId, episodeId);
        if (episode == null) { return NotFound(); }
        return Ok(episode);
    }

    /// <summary>
    /// Creates a new episode for a season. (Admin only)
    /// </summary>
    /// <param name="seasonId">The ID of the season.</param>
    /// <param name="dto">The data for the new episode.</param>
    /// <response code="201">Returns the newly created episode.</response>
    /// <response code="400">If the request body is invalid.</response>
    /// <response code="404">If the season is not found.</response>
    [HttpPost]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(typeof(EpisodeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EpisodeDto>> CreateEpisode(int seasonId, [FromBody] CreateEpisodeDto dto)
    {
        var episode = await _episodeService.CreateEpisodeAsync(seasonId, dto);
        return CreatedAtAction(nameof(GetEpisode), new { seasonId, episodeId = episode.Id }, episode);
    }

    /// <summary>
    /// Creates multiple episodes for a season in a single request. (Admin only)
    /// </summary>
    /// <param name="seasonId">The ID of the season.</param>
    /// <param name="dtos">A list of episodes to create.</param>
    /// <response code="200">Returns the list of newly created episodes.</response>
    /// <response code="400">If the request body is empty or invalid.</response>
    /// <response code="404">If the season is not found.</response>
    [HttpPost("bulk")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(typeof(List<EpisodeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<EpisodeDto>>> CreateEpisodesBulk(int seasonId, [FromBody] List<CreateEpisodeDto> dtos)
    {
        var episodes = await _episodeService.CreateEpisodesAsync(seasonId, dtos);
        return Ok(episodes);
    }

    /// <summary>
    /// Updates an existing episode. (Admin only)
    /// </summary>
    /// <param name="seasonId">The ID of the season.</param>
    /// <param name="episodeId">The ID of the episode to update.</param>
    /// <param name="dto">The updated data for the episode.</param>
    /// <response code="204">If the episode was updated successfully.</response>
    /// <response code="404">If the season or episode is not found.</response>
    [HttpPut("{episodeId}")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// Deletes an episode from a season. (Admin only)
    /// </summary>
    /// <param name="seasonId">The ID of the season.</param>
    /// <param name="episodeId">The ID of the episode to delete.</param>
    /// <response code="204">If the episode was deleted successfully.</response>
    /// <response code="404">If the season or episode is not found.</response>
    [HttpDelete("{episodeId}")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
