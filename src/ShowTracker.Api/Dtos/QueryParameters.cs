
using System.Text.Json.Serialization;
namespace ShowTracker.Api.Dtos;

public class QueryParameters<T> where T : Enum
{
    private const int MaxPageSize = 50;
    private int _pageSize = 10;

    public int Page { get; set; } = 1;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
    }

    public T? SortBy { get; set; }
    public SortOrder SortOrder { get; set; } = SortOrder.asc;
    public ExportFormat Format { get; set; } = ExportFormat.json;
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SortOrder
{
    asc,
    desc
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ExportFormat
{
    json,
    csv,
    pdf
}
