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
}
