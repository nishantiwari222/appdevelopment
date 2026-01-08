using Microsoft.EntityFrameworkCore;
using YourOwnJournal.Data;
using YourOwnJournal.Models;

namespace YourOwnJournal.Repositories;

public class JournalEntryRepository : IJournalEntryRepository
{
    private readonly AppDbContext _dbContext;

    public JournalEntryRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<JournalEntry?> GetByIdAsync(int entryId)
    {
        return await BaseQuery()
            .FirstOrDefaultAsync(e => e.EntryId == entryId);
    }

    public async Task<JournalEntry?> GetByDateAsync(DateTime date)
    {
        var normalized = date.Date;
        return await BaseQuery()
            .FirstOrDefaultAsync(e => e.EntryDate == normalized);
    }

    public async Task<IReadOnlyList<JournalEntry>> GetAllAsync()
    {
        return await BaseQuery()
            .OrderByDescending(e => e.EntryDate)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<JournalEntry>> GetPagedAsync(int pageNumber, int pageSize)
    {
        return await BaseQuery()
            .OrderByDescending(e => e.EntryDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetTotalCountAsync()
    {
        return await _dbContext.JournalEntries.CountAsync();
    }

    public async Task<IReadOnlyList<JournalEntry>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var start = startDate.Date;
        var end = endDate.Date;

        return await BaseQuery()
            .Where(e => e.EntryDate >= start && e.EntryDate <= end)
            .OrderBy(e => e.EntryDate)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<JournalEntry>> SearchAsync(string? query, DateTime? startDate, DateTime? endDate, int? moodId, IReadOnlyList<int> tagIds)
    {
        IQueryable<JournalEntry> search = BaseQuery();

        if (!string.IsNullOrWhiteSpace(query))
        {
            search = search.Where(e => e.Title.Contains(query) || e.ContentMarkdown.Contains(query));
        }

        if (startDate.HasValue)
        {
            var start = startDate.Value.Date;
            search = search.Where(e => e.EntryDate >= start);
        }

        if (endDate.HasValue)
        {
            var end = endDate.Value.Date;
            search = search.Where(e => e.EntryDate <= end);
        }

        if (moodId.HasValue)
        {
            search = search.Where(e => e.PrimaryMoodId == moodId.Value || e.SecondaryMoods.Any(sm => sm.MoodId == moodId));
        }

        if (tagIds.Count > 0)
        {
            search = search.Where(e => e.EntryTags.Any(et => tagIds.Contains(et.TagId)));
        }

        return await search
            .OrderByDescending(e => e.EntryDate)
            .ToListAsync();
    }

    public async Task AddAsync(JournalEntry entry)
    {
        _dbContext.JournalEntries.Add(entry);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(JournalEntry entry)
    {
        _dbContext.JournalEntries.Update(entry);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(JournalEntry entry)
    {
        _dbContext.JournalEntries.Remove(entry);
        await _dbContext.SaveChangesAsync();
    }

    private IQueryable<JournalEntry> BaseQuery()
    {
        return _dbContext.JournalEntries
            .Include(e => e.PrimaryMood)
            .Include(e => e.SecondaryMoods)
                .ThenInclude(sm => sm.Mood)
            .Include(e => e.EntryTags)
                .ThenInclude(et => et.Tag);
    }
}
