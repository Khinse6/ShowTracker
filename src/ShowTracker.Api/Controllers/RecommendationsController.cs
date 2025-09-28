using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShowTracker.Api.Dtos;
using ShowTracker.Api.Services;
using System.Security.Claims;

namespace ShowTracker.Api.Controllers;

[ApiController]
[Route("api/recommendations")]
[Authorize]
public class RecommendationsController : ControllerBase
{
    private readonly IRecommendationService _recommendationService;

    public RecommendationsController(IRecommendationService recommendationService)
    {
        _recommendationService = recommendationService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ShowSummaryDto>>> GetRecommendations(
        [FromQuery] string? sortBy,
        [FromQuery] bool sortAsc = true,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var recommendations = await _recommendationService.GetRecommendationsForUserAsync(userId, sortBy, sortAsc, page, pageSize);
        return Ok(recommendations);
    }
}
