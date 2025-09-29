using ShowTracker.Api.Dtos;
using ShowTracker.Api.Entities;

namespace ShowTracker.Api.Mappings;

public static class UserMappings
{
    public static UserPersonalDataDto ToPersonalDataDto(this User user)
    {
        return new UserPersonalDataDto
        {
            UserId = user.Id,
            Email = user.Email,
            DisplayName = user.DisplayName,
            AcceptedTerms = user.AcceptedTerms,
            FavoriteShows = user.FavoriteShows.Select(s => s.ToShowSummaryDto()).ToList()
        };
    }

    public static User ToEntity(this RegisterDto dto)
    {
        return new User
        {
            UserName = dto.Email,
            Email = dto.Email,
            DisplayName = dto.DisplayName,
            AcceptedTerms = dto.AcceptedTerms
        };
    }

    public static UserDto ToDto(this User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email ?? "",
            DisplayName = user.DisplayName ?? ""
        };
    }

    public static AuthResponseDto ToAuthResponseDto(this User user, string accessToken, string refreshToken)
    {
        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            User = user.ToDto()
        };
    }
}
