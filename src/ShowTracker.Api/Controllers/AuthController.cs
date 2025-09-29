using Microsoft.AspNetCore.Mvc;
using ShowTracker.Api.Dtos;
using ShowTracker.Api.Interfaces;
using ShowTracker.Api.Services;
using System.ComponentModel.DataAnnotations;

namespace ShowTracker.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="dto">The user's registration details.</param>
    /// <response code="200">Returns the newly created user's information.</response>
    /// <response code="400">If the registration details are invalid or the email is already in use.</response>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (!ModelState.IsValid) { return BadRequest(ModelState); }

        var response = await _authService.RegisterAsync(dto);
        if (response == null) { return BadRequest(new { message = "Registration failed. Email may already be in use." }); }

        return Ok(response); // This now returns AuthResponseDto
    }

    /// <summary>
    /// Logs in a user and returns user info and uthentication tokens.
    /// </summary>
    /// <param name="dto">The user's login credentials.</param>
    /// <response code="200">Returns some user info and tokens</response>
    /// <response code="401">If the login credentials are invalid.</response>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (!ModelState.IsValid) { return BadRequest(ModelState); }

        var response = await _authService.LoginAsync(dto);
        if (response == null) { return Unauthorized(new { message = "Invalid email or password." }); }

        return Ok(response);
    }

    /// <summary>
    /// Refreshes an authentication session using a refresh token.
    /// </summary>
    /// <param name="refreshToken">The current refresh token.</param>
    /// <response code="200">Returns some user info and new tokens</response>
    /// <response code="401">If the refresh token is invalid, expired, or has been reused.</response>
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody, Required] string refreshToken)
    {
        var result = await _authService.RefreshTokenAsync(refreshToken);

        return result.status switch
        {
            RefreshResultStatus.Success => Ok(result.response),
            RefreshResultStatus.Reused => Unauthorized(new { message = "Refresh token reuse detected. All sessions have been revoked. Please log in again." }),
            _ => Unauthorized(new { message = "Invalid or expired refresh token." })
        };
    }

    /// <summary>
    /// Logs out the user by revoking the provided refresh token.
    /// </summary>
    /// <param name="refreshToken">The refresh token to revoke.</param>
    /// <response code="204">If the logout was successful.</response>
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody, Required] string refreshToken)
    {
        await _authService.LogoutAsync(refreshToken);
        return NoContent();
    }
}
