using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShowTracker.Api.Services;

namespace ShowTracker.Api.Controllers;

[ApiController]
[Route("api/jobs")]
[Authorize(Roles = "admin")]
public class JobsController : ControllerBase
{
    private readonly RecommendationEmailJob _recommendationEmailJob;

    public JobsController(RecommendationEmailJob recommendationEmailJob)
    {
        _recommendationEmailJob = recommendationEmailJob;
    }

    /// <summary>
    /// Manually triggers the job to send recommendation emails to all users.
    /// </summary>
    [HttpPost("send-recommendations")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public IActionResult SendRecommendationEmails()
    {
        _ = _recommendationEmailJob.TriggerManually(HttpContext.RequestAborted);
        return Accepted(new { message = "Recommendation email job has been triggered." });
    }
}
