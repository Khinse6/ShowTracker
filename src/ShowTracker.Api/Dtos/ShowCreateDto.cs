namespace ShowTracker.Api.Dtos;

public record ShowCreateDto
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required DateOnly ReleaseDate { get; set; }
    public List<int> GenreIds { get; set; } = new();
    public List<string> FeaturedActors { get; set; } = new();
}

