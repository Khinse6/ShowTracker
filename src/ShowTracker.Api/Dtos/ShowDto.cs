using System.Text.Json.Serialization;

namespace ShowTracker.Api.Dtos;

public record class ShowSummaryDto
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required DateOnly ReleaseDate { get; set; }
    public required string Type { get; set; }
    public List<string> Genres { get; set; } = new();
}

public record class ShowDetailsDto : ShowSummaryDto
{
    public List<SeasonDto> Seasons { get; set; } = new();
    public List<string> Actors { get; set; } = new();
}

public record CreateShowDto
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required DateOnly ReleaseDate { get; set; }
    public int ShowTypeId { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ShowSortBy
{
    Title,
    ReleaseDate
}

public record UpdateShowDto
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required DateOnly ReleaseDate { get; set; }
    public int ShowTypeId { get; set; }
}
