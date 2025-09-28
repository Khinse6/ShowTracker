using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShowTracker.Api.Interfaces;

namespace ShowTracker.Api.Controllers;

[ApiController]
[Route("api/jobs")]
[Authorize(Roles = "admin")]
public class JobsController : ControllerBase
{
    private readonly IRecommendationJobService _recommendationJobService;

    public JobsController(IRecommendationJobService recommendationJobService)
    {
        _recommendationJobService = recommendationJobService;
    }

    /// <summary>
    /// Manually triggers the job to send recommendation emails to all users. ()Admin only)
    /// </summary>
    [HttpPost("send-recommendations")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> SendRecommendationEmails()
    {
        await _recommendationJobService.TriggerManually(HttpContext.RequestAborted);
        return Accepted(new { message = "Recommendation email job has been triggered." });
    }
}
