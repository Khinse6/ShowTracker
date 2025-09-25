using ShowTracker.Api.Dtos;
using ShowTracker.Api.Entities;

namespace ShowTracker.Api.Mappings;

public static class EpisodeMapper
{
	// Episode → EpisodeDto
	public static EpisodeDto ToDto(this Episode episode) =>
			new EpisodeDto
			{
				Id = episode.Id,
				Title = episode.Title,
				EpisodeNumber = episode.EpisodeNumber
			};
}
