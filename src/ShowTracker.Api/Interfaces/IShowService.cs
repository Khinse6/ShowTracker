
using ShowTracker.Api.Dtos;
using ShowTracker.Api.Entities;

namespace ShowTracker.Api.Interfaces;

public interface IShowService
{
    Task<List<ShowSummaryDto>> GetAllShowsAsync(string? genre, string? type, QueryParameters<ShowSortBy> parameters);

    Task<ShowDetailsDto?> GetShowByIdAsync(int id);

    Task<ShowSummaryDto> CreateShowAsync(CreateShowDto dto);

    Task<List<ShowSummaryDto>> CreateShowsAsync(List<CreateShowDto> dtos);

    Task UpdateShowAsync(int id, UpdateShowDto dto);
    Task DeleteShowAsync(int id);
}
