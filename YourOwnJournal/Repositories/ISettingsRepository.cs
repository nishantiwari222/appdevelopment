namespace YourOwnJournal.Repositories;

public interface ISettingsRepository
{
    Task<string?> GetValueAsync(string key);
    Task SetValueAsync(string key, string value);
}
