namespace YourOwnJournal.Services;

public class NoopPdfExportService : IPdfExportService
{
    public Task ExportHtmlToPdfAsync(string html, string outputPath)
    {
        throw new NotSupportedException("PDF export is not supported on this platform.");
    }
}
