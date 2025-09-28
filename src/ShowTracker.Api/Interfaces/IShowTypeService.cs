using ShowTracker.Api.Dtos;

namespace ShowTracker.Api.Interfaces;

public interface IShowTypeService
{
    Task<PaginatedResponseDto<ShowTypeDto>> GetAllAsync(QueryParameters<ShowTypeSortBy> parameters);
    Task<ShowTypeDto?> GetByIdAsync(int id);
    Task<ShowTypeDto> CreateAsync(CreateShowTypeDto dto);
    Task<List<ShowTypeDto>> BulkCreateAsync(List<CreateShowTypeDto> dtos);
    Task UpdateAsync(int id, UpdateShowTypeDto dto);
    Task DeleteAsync(int id);
}
