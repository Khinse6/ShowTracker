using ShowTracker.Api.Dtos;
using ShowTracker.Api.Entities;

namespace ShowTracker.Api.Mappings;

public static class SeasonMapper
{
    // Season → SeasonDto
    public static SeasonDto ToDto(this Season season) =>
            new SeasonDto
            {
                Id = season.Id,
                SeasonNumber = season.SeasonNumber,
                ReleaseDate = season.ReleaseDate,
                Episodes = season.Episodes?.Select(e => e.ToDto()).ToList() ?? new List<EpisodeDto>()
            };
}
