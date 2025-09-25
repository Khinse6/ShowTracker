namespace ShowTracker.Api.Dtos;

public record class ShowDetailsDto
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required DateOnly ReleaseDate { get; set; }
    public List<string> Genres { get; set; } = new();
    public List<SeasonDto> Seasons { get; set; } = new();
}
