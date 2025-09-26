namespace ShowTracker.Api.Entities;

public class Genre
{
    public int Id
    {
        get; set;
    }
    public required string Name
    {
        get; set;
    }
    public List<Show> Shows { get; set; } = new();
}
