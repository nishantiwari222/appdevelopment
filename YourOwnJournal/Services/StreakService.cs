using YourOwnJournal.Repositories;

namespace YourOwnJournal.Services;

public class StreakService
{
    private readonly IJournalEntryRepository _entryRepository;

    public StreakService(IJournalEntryRepository entryRepository)
    {
        _entryRepository = entryRepository;
    }

    public async Task<int> GetCurrentStreakAsync(DateTime? today = null)
    {
        var date = (today ?? DateTime.Today).Date;
        var entries = await _entryRepository.GetByDateRangeAsync(date.AddYears(-1), date);
        var entryDates = new HashSet<DateTime>(entries.Select(e => e.EntryDate));

        var streak = 0;
        var cursor = date;
        while (entryDates.Contains(cursor))
        {
            streak++;
            cursor = cursor.AddDays(-1);
        }

        return streak;
    }

    public async Task<int> GetLongestStreakAsync()
    {
        var entries = await _entryRepository.GetAllAsync();
        var dates = entries.Select(e => e.EntryDate).Distinct().OrderBy(d => d).ToList();
        if (dates.Count == 0)
        {
            return 0;
        }

        var longest = 1;
        var current = 1;
        for (var i = 1; i < dates.Count; i++)
        {
            if (dates[i] == dates[i - 1].AddDays(1))
            {
                current++;
                longest = Math.Max(longest, current);
            }
            else
            {
                current = 1;
            }
        }

        return longest;
    }

    public async Task<IReadOnlyList<DateTime>> GetMissedDaysAsync(DateTime startDate, DateTime endDate)
    {
        var start = startDate.Date;
        var end = endDate.Date;
        var entries = await _entryRepository.GetByDateRangeAsync(start, end);
        var entryDates = new HashSet<DateTime>(entries.Select(e => e.EntryDate));

        var missed = new List<DateTime>();
        for (var cursor = start; cursor <= end; cursor = cursor.AddDays(1))
        {
            if (!entryDates.Contains(cursor))
            {
                missed.Add(cursor);
            }
        }

        return missed;
    }
}
