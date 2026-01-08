using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.Net;
using System.Text;
using YourOwnJournal.Models;

namespace YourOwnJournal.Services;

public class ExportService
{
    private readonly IPdfExportService _pdfExportService;

    public ExportService(IPdfExportService pdfExportService)
    {
        _pdfExportService = pdfExportService;
    }

    public async Task ExportToPdfAsync(string filePath, IReadOnlyList<JournalEntry> entries)
    {
        if (OperatingSystem.IsMacCatalyst())
        {
            var html = BuildHtml(entries);
            await _pdfExportService.ExportHtmlToPdfAsync(html, filePath);
            return;
        }

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Content().Column(column =>
                {
                    column.Item().Text("YourOwnJournal Export").FontSize(20).Bold();
                    column.Item().Text($"Exported: {DateTime.Now:yyyy-MM-dd HH:mm}").FontSize(10);
                    column.Item().PaddingVertical(10).LineHorizontal(1);

                    foreach (var entry in entries.OrderBy(e => e.EntryDate))
                    {
                        column.Item().Text($"{entry.EntryDate:yyyy-MM-dd} — {entry.Title}").FontSize(14).Bold();
                        column.Item().Text($"Primary mood: {entry.PrimaryMood?.Name ?? "-"}").FontSize(10);
                        if (entry.SecondaryMoods.Count > 0)
                        {
                            var secondary = string.Join(", ", entry.SecondaryMoods.Select(sm => sm.Mood?.Name).Where(name => !string.IsNullOrWhiteSpace(name)));
                            column.Item().Text($"Secondary moods: {secondary}").FontSize(10);
                        }

                        if (entry.EntryTags.Count > 0)
                        {
                            var tags = string.Join(", ", entry.EntryTags.Select(et => et.Tag?.Name).Where(name => !string.IsNullOrWhiteSpace(name)));
                            column.Item().Text($"Tags: {tags}").FontSize(10);
                        }

                        column.Item().PaddingVertical(6).Text(entry.ContentMarkdown).FontSize(11);
                        column.Item().PaddingBottom(10).LineHorizontal(1);
                    }
                });
            });
        });

        document.GeneratePdf(filePath);
        await Task.CompletedTask;
    }

    private static string BuildHtml(IEnumerable<JournalEntry> entries)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html><head><meta charset='utf-8'/>");
        sb.AppendLine("<style>");
        sb.AppendLine("body{font-family:-apple-system,Helvetica,Arial,sans-serif;margin:32px;color:#1f1b16;} h1{font-size:20px;} h2{font-size:16px;margin-top:24px;} .meta{font-size:12px;color:#6f6459;} .tags{margin:6px 0;} .tag{display:inline-block;background:#f2e7dd;border-radius:999px;padding:4px 8px;margin-right:6px;font-size:11px;} .divider{border-top:1px solid #e3d7cc;margin:16px 0;} ");
        sb.AppendLine("</style></head><body>");
        sb.AppendLine("<h1>YourOwnJournal Export</h1>");
        sb.AppendLine($"<div class='meta'>Exported: {DateTime.Now:yyyy-MM-dd HH:mm}</div>");
        sb.AppendLine("<div class='divider'></div>");

        foreach (var entry in entries.OrderBy(e => e.EntryDate))
        {
            var title = WebUtility.HtmlEncode(entry.Title);
            sb.AppendLine($"<h2>{entry.EntryDate:yyyy-MM-dd} — {title}</h2>");
            sb.AppendLine($"<div class='meta'>Primary mood: {WebUtility.HtmlEncode(entry.PrimaryMood?.Name ?? "-")}</div>");

            if (entry.SecondaryMoods.Count > 0)
            {
                var secondary = string.Join(", ", entry.SecondaryMoods.Select(sm => sm.Mood?.Name).Where(name => !string.IsNullOrWhiteSpace(name)));
                sb.AppendLine($"<div class='meta'>Secondary moods: {WebUtility.HtmlEncode(secondary)}</div>");
            }

            if (entry.EntryTags.Count > 0)
            {
                sb.AppendLine("<div class='tags'>");
                foreach (var tag in entry.EntryTags.Select(et => et.Tag?.Name).Where(name => !string.IsNullOrWhiteSpace(name)))
                {
                    sb.AppendLine($"<span class='tag'>{WebUtility.HtmlEncode(tag!)}</span>");
                }
                sb.AppendLine("</div>");
            }

            var contentHtml = Markdig.Markdown.ToHtml(entry.ContentMarkdown ?? string.Empty);
            sb.AppendLine($"<div>{contentHtml}</div>");
            sb.AppendLine("<div class='divider'></div>");
        }

        sb.AppendLine("</body></html>");
        return sb.ToString();
    }
}
