namespace ShowTracker.Api.Dtos;

public record ShowUpdateDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateOnly? ReleaseDate { get; set; }
    public string? PosterUrl { get; set; }
    public List<int>? GenreIds { get; set; } = new();
    public List<string>? FeaturedActors { get; set; }
}

