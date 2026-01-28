using QuestPDF.Fluent;
using QuestPDF.Helpers;
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
                page.Size(new PageSize(210, 297, Unit.Millimetre));
                page.Margin(10, Unit.Millimetre);
                page.DefaultTextStyle(x => x.FontSize(12).LineHeight(1.4f));
                page.Content().Column(column =>
                {
                    column.Spacing(6);
                    column.Item().Text("YourOwnJournal Export").FontSize(22).Bold();
                    column.Item().Text($"Exported: {DateTime.Now:yyyy-MM-dd HH:mm}").FontSize(11);
                    column.Item().PaddingVertical(8).LineHorizontal(1);

                    foreach (var entry in entries.OrderBy(e => e.EntryDate))
                    {
                        column.Item().Text($"{entry.EntryDate:yyyy-MM-dd} — {entry.Title}").FontSize(15).Bold();
                        column.Item().Text($"Primary mood: {entry.PrimaryMood?.Name ?? "-"}").FontSize(11);
                        if (entry.SecondaryMoods.Count > 0)
                        {
                            var secondary = string.Join(", ", entry.SecondaryMoods.Select(sm => sm.Mood?.Name).Where(name => !string.IsNullOrWhiteSpace(name)));
                            column.Item().Text($"Secondary moods: {secondary}").FontSize(11);
                        }

                        if (entry.EntryTags.Count > 0)
                        {
                            var tags = string.Join(", ", entry.EntryTags.Select(et => et.Tag?.Name).Where(name => !string.IsNullOrWhiteSpace(name)));
                            column.Item().Text($"Tags: {tags}").FontSize(11);
                        }

                        column.Item().PaddingVertical(6).Text(entry.ContentMarkdown).FontSize(12);
                        column.Item().PaddingBottom(8).LineHorizontal(1);
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
        sb.AppendLine("<meta name='viewport' content='width=595, initial-scale=1'/>");
        sb.AppendLine("<style>");
        sb.AppendLine("@page{size:210mm 297mm;margin:10mm;} html,body{width:100%;} body{font-family:-apple-system,Helvetica,Arial,sans-serif;margin:0;color:#1f1b16;font-size:12pt;line-height:1.5;} h1{font-size:18pt;} h2{font-size:14pt;margin-top:18pt;} .meta{font-size:10pt;color:#6f6459;} .tags{margin:6pt 0;} .tag{display:inline-block;background:#f2e7dd;border-radius:999px;padding:3pt 7pt;margin-right:6pt;font-size:9pt;} .divider{border-top:1px solid #e3d7cc;margin:12pt 0;} ");
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
