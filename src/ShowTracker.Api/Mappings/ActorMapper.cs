using ShowTracker.Api.Dtos;
using ShowTracker.Api.Entities;

namespace ShowTracker.Api.Mappings;

public static class ActorMapper
{
    // Actor -> ActorSummaryDto
    public static ActorSummaryDto ToSummaryDto(this Actor actor)
    {
        return new ActorSummaryDto
        {
            Id = actor.Id,
            Name = actor.Name
        };
    }

    // Actor -> ActorDetailsDto
    public static ActorDetailsDto ToDetailsDto(this Actor actor)
    {
        return new ActorDetailsDto
        {
            Id = actor.Id,
            Name = actor.Name,
            Shows = actor.Shows?.Select(s => s.ToShowSummaryDto()).ToList() ?? new List<ShowSummaryDto>()
        };
    }

    // CreateActorDto -> Actor
    public static Actor ToEntity(this CreateActorDto dto)
    {
        return new Actor
        {
            Name = dto.Name
        };
    }

    // Update Actor entity from UpdateActorDto
    public static void UpdateEntity(this UpdateActorDto dto, Actor actor)
    {
        actor.Name = dto.Name;
    }
}
