namespace YourOwnJournal.Repositories;

public interface IAppLockRepository
{
    Task<string?> GetValueAsync(string key);
    Task SetValueAsync(string key, string value);
}
