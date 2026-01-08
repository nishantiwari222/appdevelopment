using YourOwnJournal.Models;
using YourOwnJournal.Repositories;

namespace YourOwnJournal.Services;

public class MoodService
{
    private readonly IMoodRepository _moodRepository;

    public MoodService(IMoodRepository moodRepository)
    {
        _moodRepository = moodRepository;
    }

    public Task<IReadOnlyList<Mood>> GetAllAsync() => _moodRepository.GetAllAsync();

    public Task<Mood?> GetByIdAsync(int moodId) => _moodRepository.GetByIdAsync(moodId);
}
