using Microsoft.AspNetCore.Mvc;
using ShowTracker.Api.Dtos;
using ShowTracker.Api.Services;

namespace ShowTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShowsController : ControllerBase
{
	private readonly IShowService _showService;

	public ShowsController(IShowService showService)
	{
		_showService = showService;
	}

	[HttpGet]
	public async Task<ActionResult<IEnumerable<ShowSummaryDto>>> GetAllShows()
	{
		var shows = await _showService.GetAllShowsAsync();
		return Ok(shows);
	}

	[HttpGet("{id}", Name = "GetShow")]
	public async Task<ActionResult<ShowDetailsDto>> GetShow(int id)
	{
		var show = await _showService.GetShowByIdAsync(id);
		return show is null ? NotFound() : Ok(show);
	}

	[HttpPost]
	public async Task<ActionResult<ShowSummaryDto>> CreateShow([FromBody] ShowCreateDto dto)
	{
		var created = await _showService.CreateShowAsync(dto);
		return CreatedAtRoute("GetShow", new { id = created.Id }, created);
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> UpdateShow(int id, [FromBody] ShowUpdateDto dto)
	{
		try
		{
			await _showService.UpdateShowAsync(id, dto);
		}
		catch (KeyNotFoundException)
		{
			return NotFound();
		}

		return NoContent();
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> DeleteShow(int id)
	{
		try
		{
			await _showService.DeleteShowAsync(id);
		}
		catch (KeyNotFoundException)
		{
			return NotFound();
		}

		return NoContent();
	}
}
