using Microsoft.AspNetCore.Identity;

namespace ShowTracker.Api.Entities;

public class User : IdentityUser
{
    public string? DisplayName { get; set; }
    public bool AcceptedTerms { get; set; } = false;
    public List<Show> FavoriteShows { get; set; } = new();
    public List<RefreshToken> RefreshTokens { get; set; } = new();
}

