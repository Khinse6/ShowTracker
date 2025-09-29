using ShowTracker.Api.Dtos;
using ShowTracker.Api.Entities;
using ShowTracker.Api.Services;

namespace ShowTracker.Api.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto?> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto?> LoginAsync(LoginDto dto);
    Task<(RefreshResultStatus status, AuthResponseDto? response)> RefreshTokenAsync(string token);
    Task LogoutAsync(string refreshToken);
    string GenerateAccessToken(User user);
}
