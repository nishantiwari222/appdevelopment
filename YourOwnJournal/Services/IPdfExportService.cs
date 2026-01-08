namespace YourOwnJournal.Services;

public interface IPdfExportService
{
    Task ExportHtmlToPdfAsync(string html, string outputPath);
}
