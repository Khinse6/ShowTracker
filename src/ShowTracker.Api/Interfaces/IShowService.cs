
using ShowTracker.Api.Dtos;

namespace ShowTracker.Api.Interfaces;

public interface IShowService
{
    Task<PaginatedResponseDto<ShowSummaryDto>> GetAllShowsAsync(string? genre, string? type, string? searchTerm, QueryParameters<ShowSortBy> parameters);

    Task<ShowDetailsDto?> GetShowByIdAsync(int id);

    Task<ShowSummaryDto> CreateShowAsync(CreateShowDto dto);

    Task<List<ShowSummaryDto>> CreateShowsAsync(List<CreateShowDto> dtos);

    Task UpdateShowAsync(int id, UpdateShowDto dto);
    Task DeleteShowAsync(int id);
}
