using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShowTracker.Api.Dtos;
using ShowTracker.Api.Services;

namespace ShowTracker.Api.Controllers;

[ApiController]
[Route("api/genres")]
[Authorize]
public class GenresController : ControllerBase
{
    private readonly IGenreService _genreService;

    public GenresController(IGenreService genreService)
    {
        _genreService = genreService;
    }

    // GET api/genres
    [HttpGet]
    public async Task<ActionResult<List<GenreDto>>> GetAllGenres(
        [FromQuery] string? sortBy,
        [FromQuery] bool descending = false,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var genres = await _genreService.GetAllGenresAsync(sortBy, descending, page, pageSize);
        return Ok(genres);
    }

    [HttpGet("{id}", Name = "GetGenre")]
    public async Task<ActionResult<GenreDto>> GetGenre(int id)
    {
        var genre = await _genreService.GetGenreByIdAsync(id);
        if (genre == null)
        {
            return NotFound();
        }
        return Ok(genre);
    }

    // POST api/genres
    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<GenreDto>> CreateGenre([FromBody] CreateGenreDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name)) { return BadRequest("Genre name is required."); }

        var created = await _genreService.CreateGenreAsync(dto);
        return CreatedAtAction(nameof(GetGenre), new { id = created.Id }, created);
    }

    [HttpPost("bulk")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<List<GenreDto>>> CreateGenresBulk([FromBody] List<CreateGenreDto> dtos)
    {
        if (dtos == null || !dtos.Any()) { return BadRequest("No genres provided for creation."); }

        var createdGenres = await _genreService.CreateGenresAsync(dtos);
        return Ok(createdGenres);
    }
}
