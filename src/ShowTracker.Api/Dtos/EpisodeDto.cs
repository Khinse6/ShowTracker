using System.Text.Json.Serialization;

namespace ShowTracker.Api.Dtos;

public record class EpisodeDto
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required int EpisodeNumber { get; set; }
    public required DateOnly ReleaseDate { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EpisodeSortBy
{
    EpisodeNumber,
    ReleaseDate
}

public record class CreateEpisodeDto
{
    public required int SeasonId { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required int EpisodeNumber { get; set; }
    public required DateOnly ReleaseDate { get; set; }
}

public record class UpdateEpisodeDto
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required int EpisodeNumber { get; set; }
    public required DateOnly ReleaseDate { get; set; }
}
