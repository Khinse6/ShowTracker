using ShowTracker.Api.Dtos;
using ShowTracker.Api.Entities;

namespace ShowTracker.Api.Mappings;

public static class EpisodeMapper
{

    // CreateEpisodeDto → Episode (for POST)
    public static Episode ToEntity(this CreateEpisodeDto dto) =>
        new Episode
        {
            SeasonId = dto.SeasonId,
            Title = dto.Title,
            Description = dto.Description,
            EpisodeNumber = dto.EpisodeNumber,
            ReleaseDate = dto.ReleaseDate
        };

    // Episode → EpisodeDto (for GET responses)
    public static EpisodeDto ToDto(this Episode episode) =>
        new EpisodeDto
        {
            Id = episode.Id,
            Title = episode.Title,
            Description = episode.Description,
            EpisodeNumber = episode.EpisodeNumber,
            ReleaseDate = episode.ReleaseDate
        };

    // UpdateEpisodeDto → Episode (for PUT)
    public static void UpdateEntity(this UpdateEpisodeDto dto, Episode episode)
    {
        episode.Title = dto.Title;
        episode.Description = dto.Description;
        episode.EpisodeNumber = dto.EpisodeNumber;
        episode.ReleaseDate = dto.ReleaseDate;
    }
}
