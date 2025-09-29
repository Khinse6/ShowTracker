using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ShowTracker.Api.Data;
using ShowTracker.Api.Dtos;
using ShowTracker.Api.Entities;
using ShowTracker.Api.Interfaces;
using ShowTracker.Api.Mappings;
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

    public async Task<AuthResponseDto?> RegisterAsync(RegisterDto dto)
    {
        var user = dto.ToEntity();
        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            // It might be useful to log result.Errors here
            return null;
        }

        await _userManager.AddToRoleAsync(user, "User");

        // After successful registration, generate tokens and log the user in.
        var accessToken = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken();
        refreshToken.UserId = user.Id;

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        return user.ToAuthResponseDto(accessToken, refreshToken.Token);
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        var result = await _signInManager.PasswordSignInAsync(dto.Email, dto.Password, false, false);
        if (!result.Succeeded) { return null; }

        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null) { return null; }

        var accessToken = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken();
        refreshToken.UserId = user.Id;

        // Save refresh token in DB
        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        return user.ToAuthResponseDto(accessToken, refreshToken.Token);
    }

    public async Task<(RefreshResultStatus status, AuthResponseDto? response)> RefreshTokenAsync(string token)
    {
        var refreshToken = _context.Set<RefreshToken>().SingleOrDefault(rt => rt.Token == token);
        if (refreshToken == null)
        {
            return (RefreshResultStatus.Invalid, null);
        }

        // Compromised reuse detection
        if (refreshToken.Revoked != null && refreshToken.ReplacedByToken != null)
        {
            RevokeAllUserRefreshTokens(refreshToken.UserId);
            await _context.SaveChangesAsync();
            return (RefreshResultStatus.Reused, null);
        }

        // Expired or revoked
        if (!refreshToken.IsActive)
        {
            return (RefreshResultStatus.Invalid, null);
        }

        // Normal refresh flow
        var user = await _userManager.FindByIdAsync(refreshToken.UserId);
        if (user == null)
        {
            return (RefreshResultStatus.Invalid, null);
        }

        refreshToken.Revoked = DateTime.UtcNow;

        var newRefreshToken = GenerateRefreshToken();
        newRefreshToken.UserId = user.Id;
        refreshToken.ReplacedByToken = newRefreshToken.Token;

        _context.RefreshTokens.Add(newRefreshToken);
        await _context.SaveChangesAsync();

        var newAccessToken = GenerateAccessToken(user);
        return (RefreshResultStatus.Success, user.ToAuthResponseDto(newAccessToken, newRefreshToken.Token));
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
