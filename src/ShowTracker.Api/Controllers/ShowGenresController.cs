using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShowTracker.Api.Services;

namespace ShowTracker.Api.Controllers;

[ApiController]
[Route("api/shows/{id}/genres")]
[Authorize]
public class ShowGenresController : ControllerBase
{
    private readonly IShowGenresService _service;

    public ShowGenresController(IShowGenresService service)
    {
        _service = service;
    }

    // GET api/shows/{id}/genres
    [HttpGet]
    public async Task<ActionResult<List<string>>> GetGenres(
        int id,
        [FromQuery] bool sortAsc = true,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var genres = await _service.GetGenresForShowAsync(id, sortAsc, page, pageSize);
            return Ok(genres);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // POST api/shows/{id}/genres/{genreId}
    [HttpPost("{genreId}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> AddGenre(int id, int genreId)
    {
        try
        {
            await _service.AddGenreToShowAsync(id, genreId);
            return NoContent();
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(new { message = e.Message });
        }
    }

    // PUT api/shows/{id}/genres
    [HttpPut]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> ReplaceGenres(int id, [FromBody] List<int> genreIds)
    {
        try
        {
            await _service.ReplaceGenresForShowAsync(id, genreIds);
            return NoContent();
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(new { message = e.Message });
        }
    }

    // DELETE api/shows/{id}/genres/{genreId}
    [HttpDelete("{genreId}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> RemoveGenre(int id, int genreId)
    {
        try
        {
            await _service.RemoveGenreFromShowAsync(id, genreId);
            return NoContent();
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(new { message = e.Message });
        }
    }


}
