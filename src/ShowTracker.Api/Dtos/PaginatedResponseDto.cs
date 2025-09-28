namespace ShowTracker.Api.Dtos;

/// <summary>
/// A generic DTO for returning paginated data from the API.
/// </summary>
/// <typeparam name="T">The type of the items in the list.</typeparam>
public class PaginatedResponseDto<T>
{
    /// <summary>
    /// The collection of items for the current page.
    /// </summary>
    public IEnumerable<T> Items { get; set; } = new List<T>();

    /// <summary>
    /// The current page number.
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// The number of items per page.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// The total number of items across all pages.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// The total number of pages.
    /// </summary>
    public int TotalPages { get; set; }
}
