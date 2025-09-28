using ShowTracker.Api.Entities;
using ShowTracker.Api.Services;

namespace ShowTracker.Api.Interfaces;

public interface IAuthService
{
    Task<User?> RegisterAsync(string email, string password, string displayName, bool acceptedTerms);
    Task<(string accessToken, string refreshToken)?> LoginAsync(string email, string password);
    Task<(RefreshResultStatus status, string? accessToken, string? refreshToken)> RefreshTokenAsync(string token);
    Task LogoutAsync(string refreshToken);
    string GenerateAccessToken(User user);
}
