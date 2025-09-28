using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ShowTracker.Api.Data;
using ShowTracker.Api.Entities;
using ShowTracker.Api.Interfaces;
using ShowTracker.Api.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace ShowTracker.Api.Services;

public enum RefreshResultStatus { Success, Invalid, Reused }

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly JwtSettings _jwtSettings;
    private readonly ShowStoreContext _context;

    public AuthService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IOptions<JwtSettings> jwtSettings,
        ShowStoreContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtSettings = jwtSettings.Value;
        _context = context;
    }

    public async Task<User?> RegisterAsync(string email, string password, string displayName, bool acceptedTerms)
    {
        var user = new User
        {
            UserName = email,
            Email = email,
            DisplayName = displayName,
            AcceptedTerms = acceptedTerms
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            return null;
        }

        await _userManager.AddToRoleAsync(user, "User");
        return user;
    }

    public async Task<(string accessToken, string refreshToken)?> LoginAsync(string email, string password)
    {
        var result = await _signInManager.PasswordSignInAsync(email, password, false, false);
        if (!result.Succeeded) { return null; }

        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) { return null; }

        var accessToken = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken();
        refreshToken.UserId = user.Id;

        // Save refresh token in DB
        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        return (accessToken, refreshToken.Token);
    }

    public async Task<(RefreshResultStatus status, string? accessToken, string? refreshToken)> RefreshTokenAsync(string token)
    {
        var refreshToken = _context.Set<RefreshToken>().SingleOrDefault(rt => rt.Token == token);
        if (refreshToken == null)
        {
            return (RefreshResultStatus.Invalid, null, null);
        }

        // Compromised reuse detection
        if (refreshToken.Revoked != null && refreshToken.ReplacedByToken != null)
        {
            RevokeAllUserRefreshTokens(refreshToken.UserId);
            await _context.SaveChangesAsync();
            return (RefreshResultStatus.Reused, null, null);
        }

        // Expired or revoked
        if (!refreshToken.IsActive)
        {
            return (RefreshResultStatus.Invalid, null, null);
        }

        // Normal refresh flow
        var user = await _userManager.FindByIdAsync(refreshToken.UserId);
        if (user == null)
        {
            return (RefreshResultStatus.Invalid, null, null);
        }

        refreshToken.Revoked = DateTime.UtcNow;

        var newRefreshToken = GenerateRefreshToken();
        newRefreshToken.UserId = user.Id;
        refreshToken.ReplacedByToken = newRefreshToken.Token;

        _context.RefreshTokens.Add(newRefreshToken);
        await _context.SaveChangesAsync();

        var newAccessToken = GenerateAccessToken(user);
        return (RefreshResultStatus.Success, newAccessToken, newRefreshToken.Token);
    }

    public async Task LogoutAsync(string refreshToken)
    {
        var token = _context.Set<RefreshToken>().SingleOrDefault(rt => rt.Token == refreshToken);

        if (token != null)
        {
            token.Revoked = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    //  Revoke all tokens for a user (on reuse detection)
    private void RevokeAllUserRefreshTokens(string userId)
    {
        var tokens = _context.Set<RefreshToken>().Where(rt => rt.UserId == userId && rt.IsActive);
        foreach (var t in tokens)
        {
            t.Revoked = DateTime.UtcNow;
        }
    }

    public string GenerateAccessToken(User user)
    {
        var userRoles = _userManager.GetRolesAsync(user).Result;

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new Claim("displayName", user.DisplayName ?? "")
        };

        foreach (var role in userRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private RefreshToken GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);

        return new RefreshToken
        {
            Token = Convert.ToBase64String(randomBytes).Replace("+", "-").Replace("/", "_"),
            Expires = DateTime.UtcNow.AddDays(7)
        };
    }
}
