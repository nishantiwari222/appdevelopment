using YourOwnJournal.Models;

namespace YourOwnJournal.Repositories;

public interface IJournalEntryRepository
{
    Task<JournalEntry?> GetByIdAsync(int entryId);
    Task<JournalEntry?> GetByDateAsync(DateTime date);
    Task<IReadOnlyList<JournalEntry>> GetAllAsync();
    Task<IReadOnlyList<JournalEntry>> GetPagedAsync(int pageNumber, int pageSize);
    Task<int> GetTotalCountAsync();
    Task<IReadOnlyList<JournalEntry>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IReadOnlyList<JournalEntry>> SearchAsync(string? query, DateTime? startDate, DateTime? endDate, int? moodId, IReadOnlyList<int> tagIds);
    Task AddAsync(JournalEntry entry);
    Task UpdateAsync(JournalEntry entry);
    Task DeleteAsync(JournalEntry entry);
}
