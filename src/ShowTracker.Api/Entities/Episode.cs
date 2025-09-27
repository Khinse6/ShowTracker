
namespace ShowTracker.Api.Entities;

public class Episode
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required int EpisodeNumber { get; set; }
    public DateOnly ReleaseDate { get; set; }
    public int SeasonId { get; set; }
    public Season Season { get; set; } = null!;
}
