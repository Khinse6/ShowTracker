using ShowTracker.Api.Dtos;
using ShowTracker.Api.Entities;

namespace ShowTracker.Api.Mappings;

public static class SeasonMapper
{
    // Create a new Season (CreateSeasonDto -> Season)
    public static Season ToEntity(this CreateSeasonDto dto)
    {
        return new Season
        {
            ShowId = dto.ShowId,
            SeasonNumber = dto.SeasonNumber,
            ReleaseDate = dto.ReleaseDate
        };
    }

    // Update an existing Season (UpdateSeasonDto -> Season)
    public static void UpdateEntity(this UpdateSeasonDto dto, Season season)
    {
        season.SeasonNumber = dto.SeasonNumber;
        season.ReleaseDate = dto.ReleaseDate;
    }

    // Map Season -> SeasonDto (for GET endpoints)
    public static SeasonDto ToDto(this Season season)
    {
        return new SeasonDto
        {
            Id = season.Id,
            SeasonNumber = season.SeasonNumber,
            ReleaseDate = season.ReleaseDate,
            Episodes = season.Episodes?.Select(e => e.ToDto()).ToList() ?? new List<EpisodeDto>()
        };
    }
}
