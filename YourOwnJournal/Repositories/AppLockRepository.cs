using Microsoft.EntityFrameworkCore;
using YourOwnJournal.Data;
using YourOwnJournal.Models;

namespace YourOwnJournal.Repositories;

public class AppLockRepository : IAppLockRepository
{
    private readonly AppDbContext _dbContext;

    public AppLockRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<string?> GetValueAsync(string key)
    {
        var record = await _dbContext.AppLocks.FirstOrDefaultAsync(s => s.Key == key);
        return record?.Value;
    }

    public async Task SetValueAsync(string key, string value)
    {
        var record = await _dbContext.AppLocks.FirstOrDefaultAsync(s => s.Key == key);
        if (record == null)
        {
            record = new AppLock { Key = key, Value = value };
            _dbContext.AppLocks.Add(record);
        }
        else
        {
            record.Value = value;
            _dbContext.AppLocks.Update(record);
        }

        await _dbContext.SaveChangesAsync();
    }
}
