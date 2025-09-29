namespace ShowTracker.Api.Dtos;

public record class UserDto
{
    public required string Id { get; set; }
    public required string Email { get; set; }
    public required string DisplayName { get; set; }
}
