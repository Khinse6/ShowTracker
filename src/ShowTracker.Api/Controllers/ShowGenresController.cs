using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShowTracker.Api.Dtos;
using ShowTracker.Api.Helpers;
using ShowTracker.Api.Services;

namespace ShowTracker.Api.Controllers;

[ApiController]
[Route("api/shows/{showId}/genres")]
[Authorize]
public class ShowGenresController : ExportableControllerBase
{
    private readonly IShowGenresService _showGenresService;

    public ShowGenresController(IShowGenresService showGenresService)
    {
        _showGenresService = showGenresService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetShowGenres(int showId, [FromQuery] QueryParameters<GenreSortBy> parameters)
    {
        try
        {
            var genres = await _showGenresService.GetGenresForShowAsync(showId, parameters);

            return CreateExportOrOkResult(genres, parameters.Format, $"Genres for Show ID: {showId}", $"show-{showId}-genres");
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Show not found.");
        }
    }

    [HttpPost("{genreId}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> AddGenreToShow(int showId, int genreId)
    {
        await _showGenresService.AddGenreToShowAsync(showId, genreId);
        return NoContent();
    }

    [HttpDelete("{genreId}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> RemoveGenreFromShow(int showId, int genreId)
    {
        await _showGenresService.RemoveGenreFromShowAsync(showId, genreId);
        return NoContent();
    }

    [HttpPut]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> ReplaceGenresForShow(int showId, [FromBody] List<int> genreIds)
    {
        await _showGenresService.ReplaceGenresForShowAsync(showId, genreIds);
        return NoContent();
    }
}
