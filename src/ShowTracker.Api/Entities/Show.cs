namespace ShowTracker.Api.Entities;

public class Show
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required DateOnly ReleaseDate { get; set; }
    public int ShowTypeId { get; set; }
    public ShowType ShowType { get; set; } = null!;
    public List<Season> Seasons { get; set; } = new();
    public List<Genre> Genres { get; set; } = new();
    public List<User> FavoritedByUsers { get; set; } = new();
    public List<Actor> Actors { get; set; } = new();
}
