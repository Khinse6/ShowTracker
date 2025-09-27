namespace ShowTracker.Api.Entities;

public class ShowType
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public List<Show> Shows { get; set; } = new();
}
