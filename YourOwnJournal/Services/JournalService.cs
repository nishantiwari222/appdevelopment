using Microsoft.Extensions.Logging;
using YourOwnJournal.Models;
using YourOwnJournal.Repositories;

namespace YourOwnJournal.Services;

public class JournalService
{
    private readonly IJournalEntryRepository _entryRepository;
    private readonly ITagRepository _tagRepository;
    private readonly ILogger<JournalService> _logger;

    public JournalService(IJournalEntryRepository entryRepository, ITagRepository tagRepository, ILogger<JournalService> logger)
    {
        _entryRepository = entryRepository;
        _tagRepository = tagRepository;
        _logger = logger;
    }

    public Task<JournalEntry?> GetByDateAsync(DateTime date) => _entryRepository.GetByDateAsync(date);

    public Task<JournalEntry?> GetByIdAsync(int entryId) => _entryRepository.GetByIdAsync(entryId);

    public Task<IReadOnlyList<JournalEntry>> GetPagedAsync(int pageNumber, int pageSize) => _entryRepository.GetPagedAsync(pageNumber, pageSize);

    public Task<int> GetTotalCountAsync() => _entryRepository.GetTotalCountAsync();

    public Task<IReadOnlyList<JournalEntry>> GetByDateRangeAsync(DateTime startDate, DateTime endDate) => _entryRepository.GetByDateRangeAsync(startDate, endDate);

    public Task<IReadOnlyList<JournalEntry>> SearchAsync(string? query, DateTime? startDate, DateTime? endDate, int? moodId, IReadOnlyList<int> tagIds)
        => _entryRepository.SearchAsync(query, startDate, endDate, moodId, tagIds);

    public async Task<(bool Success, string Message)> SaveEntryAsync(JournalEntryEditModel model)
    {
        if (model.PrimaryMoodId == null || model.PrimaryMoodId <= 0)
        {
            return (false, "Primary mood is required.");
        }

        if (model.SecondaryMoodIds.Count > 2)
        {
            return (false, "You can select up to two secondary moods.");
        }

        var existing = await _entryRepository.GetByDateAsync(model.EntryDate);
        if (existing != null && model.EntryId != existing.EntryId)
        {
            return (false, "An entry already exists for this date.");
        }

        if (model.EntryId.HasValue)
        {
            var entry = await _entryRepository.GetByIdAsync(model.EntryId.Value);
            if (entry == null)
            {
                return (false, "Entry not found.");
            }

            await MapToEntryAsync(model, entry, isNew: false);
            await _entryRepository.UpdateAsync(entry);
        }
        else
        {
            var entry = new JournalEntry();
            await MapToEntryAsync(model, entry, isNew: true);
            await _entryRepository.AddAsync(entry);
        }

        return (true, "Saved.");
    }

    public async Task DeleteEntryAsync(int entryId)
    {
        var entry = await _entryRepository.GetByIdAsync(entryId);
        if (entry == null)
        {
            _logger.LogWarning("Attempted to delete entry {EntryId} but it was not found", entryId);
            return;
        }

        await _entryRepository.DeleteAsync(entry);
    }

    private async Task MapToEntryAsync(JournalEntryEditModel model, JournalEntry entry, bool isNew)
    {
        var now = DateTime.UtcNow;
        entry.EntryDate = model.EntryDate.Date;
        entry.Title = model.Title.Trim();
        entry.ContentMarkdown = model.ContentMarkdown.Trim();
        var primaryMoodId = model.PrimaryMoodId ?? throw new InvalidOperationException("Primary mood missing.");
        entry.PrimaryMoodId = primaryMoodId;

        if (isNew)
        {
            entry.CreatedAt = now;
        }

        entry.UpdatedAt = now;

        entry.SecondaryMoods.Clear();
        foreach (var moodId in model.SecondaryMoodIds.Distinct())
        {
            entry.SecondaryMoods.Add(new EntrySecondaryMood { MoodId = moodId, EntryId = entry.EntryId });
        }

        entry.EntryTags.Clear();
        foreach (var tagName in model.Tags.Select(t => t.Trim()).Where(t => !string.IsNullOrWhiteSpace(t)).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            var tag = await _tagRepository.GetByNameAsync(tagName);
            if (tag == null)
            {
                tag = await _tagRepository.AddAsync(new Tag { Name = tagName, IsPrebuilt = false });
            }

            entry.EntryTags.Add(new EntryTag { TagId = tag.TagId, EntryId = entry.EntryId });
        }
    }
}
