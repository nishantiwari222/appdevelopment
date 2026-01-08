using Microsoft.EntityFrameworkCore;
using YourOwnJournal.Data;
using YourOwnJournal.Models;

namespace YourOwnJournal.Repositories;

public class MoodRepository : IMoodRepository
{
    private readonly AppDbContext _dbContext;

    public MoodRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Mood>> GetAllAsync()
    {
        return await _dbContext.Moods
            .OrderBy(m => m.Category)
            .ThenBy(m => m.Name)
            .ToListAsync();
    }

    public async Task<Mood?> GetByIdAsync(int moodId)
    {
        return await _dbContext.Moods.FirstOrDefaultAsync(m => m.MoodId == moodId);
    }
}
