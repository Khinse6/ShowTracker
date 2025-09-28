using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace ShowTracker.Api.Helpers.Pdf;

public class SimpleListReportDocument : IDocument
{
    private readonly IEnumerable<string> _items;
    private readonly string _title;

    public SimpleListReportDocument(IEnumerable<string> items, string title)
    {
        _items = items;
        _title = title;
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
                    .Column(column =>
                    {
                        foreach (var item in _items)
                        {
                            column.Item().Text(item);
                        }
                    });
            });
    }
}
