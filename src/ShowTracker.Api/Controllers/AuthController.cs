using Microsoft.AspNetCore.Mvc;
using ShowTracker.Api.Dtos;
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

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (!ModelState.IsValid) { return BadRequest(ModelState); }

        var user = await _authService.RegisterAsync(dto.Email, dto.Password, dto.DisplayName, dto.AcceptedTerms);
        if (user == null) { return BadRequest("Registration failed. Possibly email already in use."); }

        return Ok(new { user.Id, user.Email, user.DisplayName });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (!ModelState.IsValid) { return BadRequest(ModelState); }

        var tokens = await _authService.LoginAsync(dto.Email, dto.Password);
        if (tokens == null) { return Unauthorized("Invalid email or password."); }

        return Ok(new
        {
            AccessToken = tokens.Value.accessToken,
            RefreshToken = tokens.Value.refreshToken
        });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto dto)
    {
        var tokens = await _authService.RefreshTokenAsync(dto.Token);
        if (tokens == null) { return Unauthorized("Invalid or expired refresh token."); }

        return Ok(new
        {
            AccessToken = tokens.Value.accessToken,
            RefreshToken = tokens.Value.refreshToken
        });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenDto dto)
    {
        await _authService.LogoutAsync(dto.Token);
        return NoContent();
    }
}
