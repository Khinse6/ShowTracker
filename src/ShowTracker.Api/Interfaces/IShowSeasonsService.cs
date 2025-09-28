using ShowTracker.Api.Dtos;

namespace ShowTracker.Api.Interfaces;

public interface IShowSeasonsService
{
    Task<PaginatedResponseDto<SeasonDto>> GetSeasonsForShowAsync(int showId, QueryParameters<SeasonSortBy> parameters);
    Task<SeasonDto?> GetSeasonAsync(int showId, int seasonId);
    Task<SeasonDto> CreateSeasonAsync(CreateSeasonDto dto);
    Task<List<SeasonDto>> CreateSeasonsAsync(int showId, List<CreateSeasonDto> dtos);
    Task UpdateSeasonAsync(int showId, int seasonId, UpdateSeasonDto dto);
    Task DeleteSeasonAsync(int showId, int seasonId);
}
