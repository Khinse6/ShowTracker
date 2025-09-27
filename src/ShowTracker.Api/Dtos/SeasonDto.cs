namespace ShowTracker.Api.Dtos;

public record class SeasonDto
{
    public int Id { get; set; }
    public int SeasonNumber { get; set; }
    public DateOnly ReleaseDate { get; set; }
    public List<EpisodeDto> Episodes { get; set; } = new();
}

public record class CreateSeasonDto
{
    public required int ShowId { get; set; }
    public required int SeasonNumber { get; set; }
    public required DateOnly ReleaseDate { get; set; }
}

public record class UpdateSeasonDto
{
    public required int SeasonNumber { get; set; }
    public required DateOnly ReleaseDate { get; set; }
}
