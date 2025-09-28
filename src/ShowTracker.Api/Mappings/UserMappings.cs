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
}
