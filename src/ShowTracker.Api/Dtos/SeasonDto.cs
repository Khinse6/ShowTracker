namespace ShowTracker.Api.Dtos;

public record class SeasonDto
{
    public int Id { get; set; }
    public int SeasonNumber { get; set; }
    public DateOnly ReleaseDate { get; set; }
    public string? PosterUrl { get; set; }
    public List<EpisodeDto> Episodes { get; set; } = new();
}
