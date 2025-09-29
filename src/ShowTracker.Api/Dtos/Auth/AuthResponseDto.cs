namespace ShowTracker.Api.Dtos;

public record class AuthResponseDto
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
    public required UserDto User { get; set; } = null!;
}
