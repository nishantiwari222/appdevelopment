using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.RegularExpressions;
using YourOwnJournal.Data;

namespace YourOwnJournal.Services;

public class AnalyticsService
{
    private readonly AppDbContext _dbContext;

    public AnalyticsService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<CategoryCount>> GetMoodCategoryDistributionAsync(DateTime startDate, DateTime endDate)
    {
        var start = startDate.Date;
        var end = endDate.Date;

        return await _dbContext.JournalEntries
            .Where(e => e.EntryDate >= start && e.EntryDate <= end)
            .Join(_dbContext.Moods, e => e.PrimaryMoodId, m => m.MoodId, (e, m) => m.Category)
            .GroupBy(category => category)
            .Select(group => new CategoryCount(group.Key, group.Count()))
            .ToListAsync();
    }

    public async Task<IReadOnlyList<MoodCount>> GetTopMoodsAsync(DateTime startDate, DateTime endDate)
    {
        var start = startDate.Date;
        var end = endDate.Date;

        return await _dbContext.JournalEntries
            .Where(e => e.EntryDate >= start && e.EntryDate <= end)
            .Join(_dbContext.Moods, e => e.PrimaryMoodId, m => m.MoodId, (e, m) => m.Name)
            .GroupBy(name => name)
            .OrderByDescending(group => group.Count())
            .Select(group => new MoodCount(group.Key, group.Count()))
            .ToListAsync();
    }

    public async Task<IReadOnlyList<TagCount>> GetTagUsageAsync(DateTime startDate, DateTime endDate)
    {
        var start = startDate.Date;
        var end = endDate.Date;

        return await _dbContext.EntryTags
            .Where(et => et.Entry!.EntryDate >= start && et.Entry!.EntryDate <= end)
            .GroupBy(et => et.Tag!.Name)
            .OrderByDescending(group => group.Count())
            .Select(group => new TagCount(group.Key, group.Count()))
            .ToListAsync();
    }

    public async Task<IReadOnlyList<WordCountPoint>> GetWordCountTrendAsync(DateTime startDate, DateTime endDate)
    {
        var start = startDate.Date;
        var end = endDate.Date;

        var entries = await _dbContext.JournalEntries
            .Where(e => e.EntryDate >= start && e.EntryDate <= end)
            .OrderBy(e => e.EntryDate)
            .Select(e => new { e.EntryDate, e.ContentMarkdown })
            .ToListAsync();

        return entries
            .Select(e => new WordCountPoint(e.EntryDate, CountWords(e.ContentMarkdown)))
            .ToList();
    }

    private static int CountWords(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return 0;
        }

        var cleaned = content;
        if (cleaned.Contains('<'))
        {
            cleaned = Regex.Replace(cleaned, "<[^>]+>", " ");
            cleaned = WebUtility.HtmlDecode(cleaned);
        }

        var parts = cleaned.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        return parts.Length;
    }
}
