namespace ShowTracker.Api.Dtos;

public record class ShowSummaryDto
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public DateOnly ReleaseDate { get; set; }
    public List<string> Genres { get; set; } = new();
}
