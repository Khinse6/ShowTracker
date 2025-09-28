using CsvHelper;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using ShowTracker.Api.Helpers.Pdf;
using System.Globalization;

namespace ShowTracker.Api.Helpers;

public static class FileExportHelper
{
    public static byte[] GenerateCsv<T>(IEnumerable<T> records)
    {
        using var memoryStream = new MemoryStream();
        // Leave the stream open so we can read from it after the writer is disposed.
        using (var writer = new StreamWriter(memoryStream, leaveOpen: true))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(records);
        }
        return memoryStream.ToArray();
    }

    public static byte[] GeneratePdf<T>(IEnumerable<T> data, string title)
    {
        IDocument document;

        // We keep the specific SimpleListReportDocument for lists of strings as it's a special case.
        if (typeof(T) == typeof(string))
        {
            document = new SimpleListReportDocument(data.Cast<string>(), title);
        }
        else
        {
            document = new GenericReportDocument<T>(data, title);
        }
        return document.GeneratePdf();
    }

    public static string GetExportFileName(string baseName, string extension)
    {
        extension = extension.TrimStart('.');
        return $"{baseName}_{DateTime.UtcNow:yyyy-MM-dd_HH-mm-ss}.{extension}";
    }
}
