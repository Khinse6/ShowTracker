namespace ShowTracker.Api.Dtos;

public record ShowUpdateDto
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required DateOnly ReleaseDate { get; set; }
}

