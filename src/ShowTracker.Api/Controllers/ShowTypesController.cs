using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShowTracker.Api.Dtos;
using ShowTracker.Api.Interfaces;

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

    /// <summary>
    /// Gets a list of all show types, with optional sorting and pagination.
    /// </summary>
    /// <param name="parameters">Query parameters for sorting, pagination, and export format.</param>
    /// <response code="200">Returns a list of show types or a file if an export format is specified.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ShowTypeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] QueryParameters<ShowTypeSortBy> parameters)
    {
        var showTypes = await _showTypeService.GetAllAsync(parameters);

        return CreateExportOrOkResult(showTypes, parameters.Format, "Show Types Report", "show-types");
    }

    /// <summary>
    /// Gets a specific show type by its unique ID.
    /// </summary>
    /// <param name="id">The ID of the show type.</param>
    /// <response code="200">Returns the details of the show type.</response>
    /// <response code="404">If a show type with the specified ID is not found.</response>
    [HttpGet("{id}", Name = "GetShowType")]
    [ProducesResponseType(typeof(ShowTypeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var type = await _showTypeService.GetByIdAsync(id);
        if (type == null) { return NotFound(); }
        return Ok(type);
    }

    /// <summary>
    /// Creates a new show type. (Admin only)
    /// </summary>
    /// <param name="dto">The data for the new show type.</param>
    /// <response code="201">Returns the newly created show type.</response>
    /// <response code="400">If the request body is invalid.</response>
    [HttpPost]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(typeof(ShowTypeDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateShowTypeDto dto)
    {
        var type = await _showTypeService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = type.Id }, type);
    }

    /// <summary>
    /// Creates multiple show types in a single request. (Admin only)
    /// </summary>
    /// <param name="dtos">A list of show types to create.</param>
    /// <response code="200">Returns the list of newly created show types.</response>
    /// <response code="400">If the request body is empty or invalid.</response>
    [HttpPost("bulk")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(typeof(List<ShowTypeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> BulkCreate([FromBody] List<CreateShowTypeDto> dtos)
    {
        var types = await _showTypeService.BulkCreateAsync(dtos);
        return Ok(types);
    }

    /// <summary>
    /// Updates an existing show type. (Admin only)
    /// </summary>
    /// <param name="id">The ID of the show type to update.</param>
    /// <param name="dto">The updated data for the show type.</param>
    /// <response code="204">If the show type was updated successfully.</response>
    /// <response code="404">If a show type with the specified ID is not found.</response>
    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// Deletes a show type. (Admin only)
    /// </summary>
    /// <param name="id">The ID of the show type to delete.</param>
    /// <response code="204">If the show type was deleted successfully.</response>
    /// <response code="404">If a show type with the specified ID is not found.</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
