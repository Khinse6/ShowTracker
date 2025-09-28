using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShowTracker.Api.Dtos;
using ShowTracker.Api.Interfaces;

namespace ShowTracker.Api.Controllers;

[ApiController]
[Route("api/genres")]
[Authorize]
public class GenresController : ExportableControllerBase
{
    private readonly IGenreService _genreService;

    public GenresController(IGenreService genreService)
    {
        _genreService = genreService;
    }

    /// <summary>
    /// Gets a list of all genres, with optional sorting and pagination. Can also export the list.
    /// </summary>
    /// <param name="parameters">Query parameters for sorting, pagination, and export format.</param>
    /// <response code="200">Returns a list of genres or a file if an export format is specified.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GenreDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllGenres(
        [FromQuery] QueryParameters<GenreSortBy> parameters)
    {
        var genres = await _genreService.GetAllGenresAsync(parameters);

        return CreateExportOrOkResult(genres, parameters.Format, "Genres Report", "genres");
    }

    /// <summary>
    /// Gets a specific genre by its unique ID.
    /// </summary>
    /// <param name="id">The ID of the genre.</param>
    /// <response code="200">Returns the details of the genre.</response>
    /// <response code="404">If a genre with the specified ID is not found.</response>
    [HttpGet("{id}", Name = "GetGenre")]
    [ProducesResponseType(typeof(GenreDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var genre = await _genreService.GetGenreByIdAsync(id);
        if (genre == null)
        {
            return NotFound();
        }
        return Ok(genre);
    }

    /// <summary>
    /// Creates a new genre. (Admin only)
    /// </summary>
    /// <param name="dto">The data for the new genre.</param>
    /// <response code="201">Returns the newly created genre.</response>
    /// <response code="400">If the request body is invalid.</response>
    [HttpPost]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(typeof(GenreDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateGenreDto dto)
    {
        var genre = await _genreService.CreateGenreAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = genre.Id }, genre);
    }

    /// <summary>
    /// Updates an existing genre. (Admin only)
    /// </summary>
    /// <param name="id">The ID of the genre to update.</param>
    /// <param name="dto">The updated data for the genre.</param>
    /// <response code="204">If the genre was updated successfully.</response>
    /// <response code="400">If the request body is invalid.</response>
    /// <response code="404">If a genre with the specified ID is not found.</response>
    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateGenreDto dto)
    {
        try
        {
            await _genreService.UpdateGenreAsync(id, dto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Deletes a genre. (Admin only)
    /// </summary>
    /// <param name="id">The ID of the genre to delete.</param>
    /// <response code="204">If the genre was deleted successfully.</response>
    /// <response code="404">If a genre with the specified ID is not found.</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _genreService.DeleteGenreAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
