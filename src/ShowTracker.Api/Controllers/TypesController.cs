using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShowTracker.Api.Dtos;
using ShowTracker.Api.Services;

namespace ShowTracker.Api.Controllers;

[ApiController]
[Route("api/types")]
[Authorize]
public class ShowTypesController : ControllerBase
{
    private readonly IShowTypeService _service;

    public ShowTypesController(IShowTypeService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? sortBy,
        [FromQuery] bool sortAsc = true,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var types = await _service.GetAllAsync(sortBy, sortAsc, page, pageSize);
        return Ok(types);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var type = await _service.GetByIdAsync(id);
        if (type == null) { return NotFound(); }
        return Ok(type);
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Create([FromBody] CreateShowTypeDto dto)
    {
        var type = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = type.Id }, type);
    }

    [HttpPost("bulk")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> BulkCreate([FromBody] List<CreateShowTypeDto> dtos)
    {
        var types = await _service.BulkCreateAsync(dtos);
        return Ok(types);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateShowTypeDto dto)
    {
        try
        {
            await _service.UpdateAsync(id, dto);
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
            await _service.DeleteAsync(id);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        return NoContent();
    }
}
