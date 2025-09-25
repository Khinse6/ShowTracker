using ShowTracker.Api.Dtos;
using ShowTracker.Api.Entities;

namespace ShowTracker.Api.Mappings;

public static class ShowMapper
{
	// Show → ShowDto
	public static ShowDto ToDto(this Show show) =>
			new ShowDto
			{
				Id = show.Id,
				Title = show.Title,
				Description = show.Description,
				ReleaseDate = show.ReleaseDate,
				Genres = show.Genres?.Select(g => g.Name).ToList() ?? new List<string>(),
				Seasons = show.Seasons?.Select(s => s.ToDto()).ToList() ?? new List<SeasonDto>()
			};

	// Show → ShowSummaryDto
	public static ShowSummaryDto ToSummaryDto(this Show show) =>
				new ShowSummaryDto
				{
					Id = show.Id,
					Title = show.Title,
					Description = show.Description,
					Genres = show.Genres?.Select(g => g.Name).ToList() ?? new List<string>()
				};

	// ShowCreateDto → Show
	public static Show ToEntity(this ShowCreateDto dto) =>
			new Show
			{
				Title = dto.Title,
				Description = dto.Description,
				ReleaseDate = dto.ReleaseDate
			};

	// ShowUpdateDto → Show (manual patching)
	public static void UpdateEntity(this ShowUpdateDto dto, Show show)
	{
		if (dto.Title != null)
			show.Title = dto.Title;

		if (dto.Description != null)
			show.Description = dto.Description;

		if (dto.ReleaseDate is DateOnly releaseDate)
			show.ReleaseDate = releaseDate;
	}
}
