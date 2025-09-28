using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShowTracker.Api.Dtos;
using ShowTracker.Api.Helpers;
using ShowTracker.Api.Interfaces;
using ShowTracker.Api.Services;
using System.Security.Claims;

namespace ShowTracker.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ExportableControllerBase
{
    private readonly IFavoritesService _favoritesService;
    private readonly IRecommendationService _recommendationService;

    public UsersController(IFavoritesService favoritesService, IRecommendationService recommendationService)
    {
        _favoritesService = favoritesService;
        _recommendationService = recommendationService;
    }

    private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet("me/favorites")]
    [ProducesResponseType(typeof(IEnumerable<ShowSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyFavorites(
        [FromQuery] QueryParameters<ShowSortBy> parameters)
    {
        var favorites = await _favoritesService.GetFavoritesAsync(GetUserId(), parameters);

        return CreateExportOrOkResult(favorites, parameters.Format, "My Favorites", "my-favorites");
    }

    [HttpPost("me/favorites/{showId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddFavorite(int showId)
    {
        try
        {
            await _favoritesService.AddFavoriteAsync(GetUserId(), showId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("me/favorites/{showId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveFavorite(int showId)
    {
        try
        {
            await _favoritesService.RemoveFavoriteAsync(GetUserId(), showId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("me/recommendations")]
    [ProducesResponseType(typeof(IEnumerable<ShowSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyRecommendations(
        [FromQuery] QueryParameters<ShowSortBy> parameters)
    {
        var recommendations = await _recommendationService.GetRecommendationsForUserAsync(GetUserId(), parameters);

        return CreateExportOrOkResult(recommendations, parameters.Format, "My Recommendations", "my-recommendations");
    }
}
