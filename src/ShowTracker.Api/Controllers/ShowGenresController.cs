using Microsoft.AspNetCore.Mvc;
using ShowTracker.Api.Services;

namespace ShowTracker.Api.Controllers;

[ApiController]
[Route("api/shows/{showId}/genres")]
public class ShowGenresController : ControllerBase
{
    private readonly IShowGenresService _service;

    public ShowGenresController(IShowGenresService service)
    {
        _service = service;
    }

    // GET api/shows/{showId}/genres
    [HttpGet]
    public async Task<ActionResult<List<string>>> GetGenres(int showId)
    {
        try
        {
            var genres = await _service.GetGenresForShowAsync(showId);
            return Ok(genres);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // POST api/shows/{showId}/genres/{genreId}
    [HttpPost("{genreId}")]
    public async Task<IActionResult> AddGenre(int showId, int genreId)
    {
        try
        {
            await _service.AddGenreToShowAsync(showId, genreId);
            return NoContent();
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(new { message = e.Message });
        }
    }

    // PUT api/shows/{showId}/genres
    [HttpPut]
    public async Task<IActionResult> ReplaceGenres(int showId, [FromBody] List<int> genreIds)
    {
        try
        {
            await _service.ReplaceGenresForShowAsync(showId, genreIds);
            return NoContent();
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(new { message = e.Message });
        }
    }

    // DELETE api/shows/{showId}/genres/{genreId}
    [HttpDelete("{genreId}")]
    public async Task<IActionResult> RemoveGenre(int showId, int genreId)
    {
        try
        {
            await _service.RemoveGenreFromShowAsync(showId, genreId);
            return NoContent();
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(new { message = e.Message });
        }
    }


}
