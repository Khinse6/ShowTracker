using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShowTracker.Api.Dtos;
using ShowTracker.Api.Entities;
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
    public async Task<ActionResult<List<string>>> GetAllGenres()
    {
        var genres = await _genreService.GetAllGenresAsync();
        return Ok(genres);
    }

    // POST api/genres
    [HttpPost]
    public async Task<ActionResult> CreateGenre([FromBody] CreateGenreDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name)) { return BadRequest("Genre name is required."); }

        var created = await _genreService.CreateGenreAsync(dto);
        return CreatedAtAction(nameof(GetAllGenres), new { id = created.Id }, created);
    }

    [HttpPost("bulk")]
    public async Task<ActionResult<List<Genre>>> CreateGenresBulk([FromBody] List<CreateGenreDto> dtos)
    {
        if (dtos == null || !dtos.Any()) { return BadRequest("No genres provided for creation."); }

        var createdGenres = await _genreService.CreateGenresAsync(dtos);
        return Ok(createdGenres);
    }

}
