using ShowTracker.Api.Entities;

namespace ShowTracker.Api.Mappings;

public static class ShowTypeMapper
{
    // ShowType -> ShowTypeDto
    public static ShowTypeDto ToDto(this ShowType entity) =>
        new ShowTypeDto
        {
            Id = entity.Id,
            Name = entity.Name
        };

    // CreateShowTypeDto -> ShowType
    public static ShowType ToEntity(this CreateShowTypeDto dto) =>
        new ShowType { Name = dto.Name };

    // UpdateShowTypeDto -> update existing ShowType entity
    public static void UpdateEntity(this UpdateShowTypeDto dto, ShowType entity)
    {
        entity.Name = dto.Name;
    }
}
