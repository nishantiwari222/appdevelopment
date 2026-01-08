using YourOwnJournal.Models;

namespace YourOwnJournal.Repositories;

public interface IMoodRepository
{
    Task<IReadOnlyList<Mood>> GetAllAsync();
    Task<Mood?> GetByIdAsync(int moodId);
}
