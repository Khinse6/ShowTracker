using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShowTracker.Api.Dtos;
using ShowTracker.Api.Interfaces;
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

    /// <summary>
    /// Gets the current user's list of favorite shows.
    /// </summary>
    /// <param name="parameters">Query parameters for sorting, pagination, and export format.</param>
    /// <response code="200">Returns a list of the user's favorite shows or a file if an export format is specified.</response>
    [HttpGet("me/favorites")]
    [ProducesResponseType(typeof(IEnumerable<ShowSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyFavorites(
        [FromQuery] QueryParameters<ShowSortBy> parameters)
    {
        var favorites = await _favoritesService.GetFavoritesAsync(GetUserId(), parameters);

        return CreateExportOrOkResult(favorites, parameters.Format, "My Favorites", "my-favorites");
    }

    /// <summary>
    /// Adds a show to the current user's favorites.
    /// </summary>
    /// <param name="showId">The ID of the show to add to favorites.</param>
    /// <response code="204">If the show was successfully added to favorites.</response>
    /// <response code="404">If the show with the specified ID is not found.</response>
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

    /// <summary>
    /// Removes a show from the current user's favorites.
    /// </summary>
    /// <param name="showId">The ID of the show to remove from favorites.</param>
    /// <response code="204">If the show was successfully removed from favorites.</response>
    /// <response code="404">If the show with the specified ID is not found.</response>
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

    /// <summary>
    /// Gets a list of recommended shows for the current user based on their favorites.
    /// </summary>
    /// <param name="parameters">Query parameters for sorting, pagination, and export format.</param>
    /// <response code="200">Returns a list of recommended shows or a file if an export format is specified.</response>
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
