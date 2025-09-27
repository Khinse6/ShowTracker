
namespace ShowTracker.Api.Dtos;

public record class GenreDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

public record CreateGenreDto
{
    public required string Name { get; set; }
}
