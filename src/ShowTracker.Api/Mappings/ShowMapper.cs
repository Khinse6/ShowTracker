using ShowTracker.Api.Dtos;
using ShowTracker.Api.Entities;

namespace ShowTracker.Api.Mappings;

public static class ShowMapper
{
	// Create a New Show (ShowCreateDto -> Show)
	public static Show ToEntity(this ShowCreateDto dto)
	{
		return new Show()
		{
			Title = dto.Title,
			Description = dto.Description,
			ReleaseDate = dto.ReleaseDate
		};
	}

	// Update an Existing Show (ShowUpdateDto -> Show)
	public static void UpdateEntity(this ShowUpdateDto dto, Show show)
	{
		show.Title = dto.Title;
		show.Description = dto.Description;
		show.ReleaseDate = dto.ReleaseDate;
	}

	// Get all Shows (Show -> ShowSummaryDto)
	public static ShowSummaryDto ToShowSummaryDto(this Show show)
	{
		return new ShowSummaryDto()
		{
			Id = show.Id,
			Title = show.Title,
			Description = show.Description,
			ReleaseDate = show.ReleaseDate,
			Genres = show.Genres?.Select(g => g.Name).ToList() ?? new List<string>()
		};
	}

	// Get a single Show (Show -> ShowDetailsDto)
	public static ShowDetailsDto ToShowDetailsDto(this Show show)
	{
		return new ShowDetailsDto()
		{
			Id = show.Id,
			Title = show.Title,
			Description = show.Description,
			ReleaseDate = show.ReleaseDate,
			Genres = show.Genres?.Select(g => g.Name).ToList() ?? new List<string>(),
			Seasons = show.Seasons?.Select(se => se.ToDto()).ToList() ?? new List<SeasonDto>()
		};
	}
}
