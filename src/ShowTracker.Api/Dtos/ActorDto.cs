using System.Text.Json.Serialization;

namespace ShowTracker.Api.Dtos;


public record class ActorSummaryDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

public record class ActorDetailsDto : ActorSummaryDto
{
    public List<string> Shows { get; set; } = new();
}

public record class CreateActorDto
{
    public required string Name { get; set; }
}

public record class UpdateActorDto
{
    public required string Name { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ActorSortBy
{
    Name
}
