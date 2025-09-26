namespace ShowTracker.Api.Dtos;

public record class RegisterDto
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string DisplayName { get; set; }
    public bool AcceptedTerms { get; set; }
}
