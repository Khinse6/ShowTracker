using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.Collections;
using System.Reflection;

namespace ShowTracker.Api.Helpers.Pdf;

public class GenericReportDocument<T> : IDocument
{
    private readonly IEnumerable<T> _data;
    private readonly string _title;
    private readonly PropertyInfo[] _properties;

    public GenericReportDocument(IEnumerable<T> data, string title)
    {
        _data = data;
        _title = title;
        _properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container
            .Page(page =>
            {
                page.Margin(50);

                page.Header()
                    .Text(_title)
                    .SemiBold().FontSize(20).FontColor("#222");

                page.Content()
                    .Table(table =>
                    {
                        // Define columns based on the number of properties
                        table.ColumnsDefinition(columns =>
                        {
                            foreach (var _ in _properties)
                            {
                                columns.RelativeColumn();
                            }
                        });

                        // Create the table header from property names
                        table.Header(header =>
                        {
                            foreach (var prop in _properties)
                            {
                                header.Cell().Text(prop.Name).SemiBold();
                            }
                        });

                        // Populate the table with data
                        foreach (var item in _data)
                        {
                            foreach (var prop in _properties)
                            {
                                var value = prop.GetValue(item);
                                var text = value is IEnumerable and not string
                                    ? string.Join(", ", (value as IEnumerable)!.Cast<object>())
                                    : value?.ToString() ?? string.Empty;

                                table.Cell().Text(text);
                            }
                        }
                    });
            });
    }
}
