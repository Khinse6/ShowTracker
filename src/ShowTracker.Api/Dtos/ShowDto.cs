namespace ShowTracker.Api.Dtos;

public record class ShowDto
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public DateOnly ReleaseDate { get; set; }
    public string? PosterUrl { get; set; }
    public List<string> FeaturedActors { get; set; } = new();
    public List<SeasonDto> Seasons { get; set; } = new();
    public List<string> Genres { get; set; } = new();
}
