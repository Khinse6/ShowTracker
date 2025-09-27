
namespace ShowTracker.Api.Dtos;

public record class GenreDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

public record class CreateGenreDto
{
    public required string Name { get; set; }
}
public record class UpdateGenreDto
{
    public required string Name { get; set; }
}
