using ShowTracker.Api.Dtos;

namespace ShowTracker.Api.Services;

public interface IShowService
{
	Task<IEnumerable<ShowSummaryDto>> GetAllShowsAsync();
	Task<ShowDetailsDto?> GetShowByIdAsync(int id);
	Task<ShowSummaryDto> CreateShowAsync(ShowCreateDto dto);
	Task UpdateShowAsync(int id, ShowUpdateDto dto);
	Task DeleteShowAsync(int id);
}
