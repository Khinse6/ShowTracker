using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShowTracker.Api.Dtos;
using ShowTracker.Api.Services;
using System.Security.Claims;

namespace ShowTracker.Api.Controllers;

[ApiController]
[Route("api/favorites")]
[Authorize]
public class FavoritesController : ControllerBase
{
    private readonly IFavoritesService _favoritesService;

    public FavoritesController(IFavoritesService favoritesService)
    {
        _favoritesService = favoritesService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ShowSummaryDto>>> GetFavorites(
        [FromQuery] string? sortBy,
        [FromQuery] bool sortAsc = true,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        // The service now requires the new parameters for sorting and pagination.
        var favorites = await _favoritesService.GetFavoritesAsync(userId!, sortBy, sortAsc, page, pageSize);
        return Ok(favorites);
    }

    [HttpPost("{showId}")]
    public async Task<IActionResult> AddFavorite(int showId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        await _favoritesService.AddFavoriteAsync(userId!, showId);
        return NoContent();
    }

    [HttpDelete("{showId}")]
    public async Task<IActionResult> RemoveFavorite(int showId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        await _favoritesService.RemoveFavoriteAsync(userId!, showId);
        return NoContent();
    }
}
