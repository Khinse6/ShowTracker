using ShowTracker.Api.Dtos;

namespace ShowTracker.Api.Interfaces;

public interface IShowEpisodesService
{
    Task<List<EpisodeDto>> GetEpisodesForSeasonAsync(int seasonId, QueryParameters<EpisodeSortBy> parameters);
    Task<EpisodeDto?> GetEpisodeAsync(int seasonId, int episodeId);
    Task<EpisodeDto> CreateEpisodeAsync(int seasonId, CreateEpisodeDto dto);
    Task<List<EpisodeDto>> CreateEpisodesAsync(int seasonId, List<CreateEpisodeDto> dtos);
    Task UpdateEpisodeAsync(int seasonId, int episodeId, UpdateEpisodeDto dto);
    Task DeleteEpisodeAsync(int seasonId, int episodeId);
}
