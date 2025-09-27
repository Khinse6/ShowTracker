namespace ShowTracker.Api.Dtos;

public record class ActorSummaryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}

public record class ActorDetailsDto : ActorSummaryDto
{
    public List<ShowSummaryDto> Shows { get; set; } = new();
}

public record class CreateActorDto
{
    public string Name { get; set; } = null!;
}

public record class UpdateActorDto
{
    public string Name { get; set; } = null!;
}
