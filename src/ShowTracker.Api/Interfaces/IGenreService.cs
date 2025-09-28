using ShowTracker.Api.Dtos;

namespace ShowTracker.Api.Interfaces;

public interface IGenreService
{
    Task<List<GenreDto>> GetAllGenresAsync(QueryParameters<GenreSortBy> parameters);
    Task<GenreDto?> GetGenreByIdAsync(int id);
    Task<GenreDto> CreateGenreAsync(CreateGenreDto dto);
    Task<List<GenreDto>> CreateGenresAsync(List<CreateGenreDto> dtos);
    Task UpdateGenreAsync(int id, UpdateGenreDto dto);
    Task DeleteGenreAsync(int id);
}
