using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShowTracker.Api.Dtos;
using ShowTracker.Api.Helpers;
using ShowTracker.Api.Services;

namespace ShowTracker.Api.Controllers;

[ApiController]
[Route("api/showtypes")]
[Authorize]
public class ShowTypesController : ExportableControllerBase
{
    private readonly IShowTypeService _showTypeService;

    public ShowTypesController(IShowTypeService showTypeService)
    {
        _showTypeService = showTypeService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ShowTypeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] QueryParameters<ShowTypeSortBy> parameters)
    {
        var showTypes = await _showTypeService.GetAllAsync(parameters);

        return CreateExportOrOkResult(showTypes, parameters.Format, "Show Types Report", "show-types");
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var type = await _showTypeService.GetByIdAsync(id);
        if (type == null) { return NotFound(); }
        return Ok(type);
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Create([FromBody] CreateShowTypeDto dto)
    {
        var type = await _showTypeService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = type.Id }, type);
    }

    [HttpPost("bulk")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> BulkCreate([FromBody] List<CreateShowTypeDto> dtos)
    {
        var types = await _showTypeService.BulkCreateAsync(dtos);
        return Ok(types);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateShowTypeDto dto)
    {
        try
        {
            await _showTypeService.UpdateAsync(id, dto);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _showTypeService.DeleteAsync(id);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        return NoContent();
    }
}
