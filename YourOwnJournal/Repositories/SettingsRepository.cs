using Microsoft.EntityFrameworkCore;
using YourOwnJournal.Data;
using YourOwnJournal.Models;

namespace YourOwnJournal.Repositories;

public class SettingsRepository : ISettingsRepository
{
    private readonly AppDbContext _dbContext;

    public SettingsRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<string?> GetValueAsync(string key)
    {
        var setting = await _dbContext.AppSettings.FirstOrDefaultAsync(s => s.Key == key);
        return setting?.Value;
    }

    public async Task SetValueAsync(string key, string value)
    {
        var setting = await _dbContext.AppSettings.FirstOrDefaultAsync(s => s.Key == key);
        if (setting == null)
        {
            setting = new AppSetting { Key = key, Value = value };
            _dbContext.AppSettings.Add(setting);
        }
        else
        {
            setting.Value = value;
            _dbContext.AppSettings.Update(setting);
        }

        await _dbContext.SaveChangesAsync();
    }
}
