namespace ShowTracker.Api.Entities;

public class Season
{
    public int Id { get; set; }
    public int SeasonNumber { get; set; }
    public DateOnly ReleaseDate { get; set; }
    public int ShowId { get; set; }
    public Show Show { get; set; } = null!;
    public List<Episode> Episodes { get; set; } = new();
}
