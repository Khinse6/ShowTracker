using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShowTracker.Api.Dtos;
using ShowTracker.Api.Interfaces;

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

    /// <summary>
    /// Gets all genres for a specific show.
    /// </summary>
    /// <param name="showId">The ID of the show.</param>
    /// <param name="parameters">Query parameters for sorting, pagination, and export format.</param>
    /// <response code="200">Returns the list of genres for the show.</response>
    /// <response code="404">If the show with the specified ID is not found.</response>
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

    /// <summary>
    /// Adds a genre to a specific show. (Admin only)
    /// </summary>
    /// <param name="showId">The ID of the show.</param>
    /// <param name="genreId">The ID of the genre to add.</param>
    /// <response code="204">If the genre was successfully added to the show.</response>
    /// <response code="404">If the show or genre is not found.</response>
    [HttpPost("{genreId}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> AddGenreToShow(int showId, int genreId)
    {
        await _showGenresService.AddGenreToShowAsync(showId, genreId);
        return NoContent();
    }

    /// <summary>
    /// Removes a genre from a specific show. (Admin only)
    /// </summary>
    /// <param name="showId">The ID of the show.</param>
    /// <param name="genreId">The ID of the genre to remove.</param>
    /// <response code="204">If the genre was successfully removed from the show.</response>
    /// <response code="404">If the show is not found.</response>
    [HttpDelete("{genreId}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> RemoveGenreFromShow(int showId, int genreId)
    {
        await _showGenresService.RemoveGenreFromShowAsync(showId, genreId);
        return NoContent();
    }

    /// <summary>
    /// Replaces all genres for a specific show with a new set of genres. (Admin only)
    /// </summary>
    /// <param name="showId">The ID of the show to update.</param>
    /// <param name="genreIds">A list of genre IDs to set for the show. Any existing genres will be removed.</param>
    /// <response code="204">If the genres were successfully replaced.</response>
    /// <response code="404">If the show is not found.</response>
    [HttpPut]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> ReplaceGenresForShow(int showId, [FromBody] List<int> genreIds)
    {
        await _showGenresService.ReplaceGenresForShowAsync(showId, genreIds);
        return NoContent();
    }
}
