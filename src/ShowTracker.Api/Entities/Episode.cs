
namespace ShowTracker.Api.Entities;

public class Episode
{
    public int Id
    {
        get; set;
    }
    public string Title { get; set; } = null!;
    public string? Description
    {
        get; set;
    }
    public int EpisodeNumber
    {
        get; set;
    }
    public DateOnly ReleaseDate
    {
        get; set;
    }
    public int SeasonId
    {
        get; set;
    }
    public Season Season { get; set; } = null!;
}
