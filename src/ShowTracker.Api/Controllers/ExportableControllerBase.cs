using Microsoft.AspNetCore.Mvc;
using ShowTracker.Api.Dtos;
using ShowTracker.Api.Helpers;

namespace ShowTracker.Api.Controllers;

[ApiController]
public abstract class ExportableControllerBase : Controller
{
    protected IActionResult CreateExportOrOkResult<T>(
        IEnumerable<T> data,
        ExportFormat format,
        string reportTitle,
        string baseFileName)
    {
        switch (format)
        {
            case ExportFormat.pdf:
                var pdfBytes = FileExportHelper.GeneratePdf(data, reportTitle);
                return File(pdfBytes, "application/pdf", fileDownloadName: FileExportHelper.GetExportFileName(baseFileName, "pdf"));
            case ExportFormat.csv:
                var csvBytes = FileExportHelper.GenerateCsv(data);
                return File(csvBytes, "text/csv", fileDownloadName: FileExportHelper.GetExportFileName(baseFileName, "csv"));
            default: // ExportFormat.Json
                return Ok(data);
        }
    }

    protected IActionResult CreateExportOrOkResult(
        IEnumerable<string> data,
        ExportFormat format,
        string reportTitle,
        string baseFileName)
    {
        switch (format)
        {
            case ExportFormat.pdf:
                var pdfBytes = FileExportHelper.GeneratePdf(data, reportTitle);
                return File(pdfBytes, "application/pdf", fileDownloadName: FileExportHelper.GetExportFileName(baseFileName, "pdf"));
            case ExportFormat.csv:
                // Special handling for list of strings to give the CSV a header
                var csvData = data.Select(s => new { Name = s });
                var csvBytes = FileExportHelper.GenerateCsv(csvData);
                return File(csvBytes, "text/csv", fileDownloadName: FileExportHelper.GetExportFileName(baseFileName, "csv"));
            default: // ExportFormat.Json
                return Ok(data);
        }
    }
}
