using ShowTracker.Api.Dtos;
using ShowTracker.Api.Entities;

namespace ShowTracker.Api.Mappings;

public static class GenreMapper
{
    // Genre → GenreDto
    public static GenreDto ToDto(this Genre genre) =>
        new GenreDto
        {
            Id = genre.Id,
            Name = genre.Name
        };

    // CreateGenreDto → Genre
    public static Genre ToEntity(this CreateGenreDto dto) =>
        new Genre
        {
            Name = dto.Name
        };

    // UpdateGenreDto → update existing Genre entity
    public static void UpdateEntity(this UpdateGenreDto dto, Genre genre)
    {
        genre.Name = dto.Name;
    }
}
