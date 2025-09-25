namespace ShowTracker.Api.Dtos;

public record class EpisodeDto
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public int EpisodeNumber { get; set; }
    public DateOnly ReleaseDate { get; set; }
    public string? Duration { get; set; }
    public string? ThumbnailUrl { get; set; }
}

