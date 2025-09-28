namespace ShowTracker.Api.Dtos;

public class UserPersonalDataDto
{
    public string UserId { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? DisplayName { get; set; }
    public bool AcceptedTerms { get; set; }
    public List<ShowSummaryDto> FavoriteShows { get; set; } = new();
}
