using Microsoft.AspNetCore.Mvc;
using ShowTracker.Api.Dtos;
using ShowTracker.Api.Interfaces;
using ShowTracker.Api.Services;

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

        var user = await _authService.RegisterAsync(dto.Email, dto.Password, dto.DisplayName, dto.AcceptedTerms);
        if (user == null) { return BadRequest("Registration failed. Email may already be in use."); }

        return Ok(new { user.Id, user.Email, user.DisplayName });
    }

    /// <summary>
    /// Logs in a user and returns authentication tokens.
    /// </summary>
    /// <param name="dto">The user's login credentials.</param>
    /// <response code="200">Returns the JWT and refresh tokens.</response>
    /// <response code="401">If the login credentials are invalid.</response>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (!ModelState.IsValid) { return BadRequest(ModelState); }

        var tokens = await _authService.LoginAsync(dto.Email, dto.Password);
        if (tokens == null) { return Unauthorized("Invalid email or password."); }

        return Ok(new
        {
            tokens.Value.accessToken,
            tokens.Value.refreshToken
        });
    }

    /// <summary>
    /// Refreshes an authentication session using a refresh token.
    /// </summary>
    /// <param name="dto">The current refresh token.</param>
    /// <response code="200">Returns a new set of JWT and refresh tokens.</response>
    /// <response code="401">If the refresh token is invalid, expired, or has been reused.</response>
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto dto)
    {
        var result = await _authService.RefreshTokenAsync(dto.Token);

        return result.status switch
        {
            RefreshResultStatus.Success => Ok(new { result.accessToken, result.refreshToken }),
            RefreshResultStatus.Reused => Unauthorized("Refresh token reuse detected. All sessions have been revoked. Please log in again."),
            _ => Unauthorized("Invalid or expired refresh token.")
        };
    }

    /// <summary>
    /// Logs out the user by revoking the provided refresh token.
    /// </summary>
    /// <param name="dto">The refresh token to revoke.</param>
    /// <response code="204">If the logout was successful.</response>
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenDto dto)
    {
        await _authService.LogoutAsync(dto.Token);
        return NoContent();
    }
}
