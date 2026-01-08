using YourOwnJournal.Models;

namespace YourOwnJournal.Repositories;

public interface ITagRepository
{
    Task<IReadOnlyList<Tag>> GetAllAsync();
    Task<Tag?> GetByNameAsync(string name);
    Task<Tag> AddAsync(Tag tag);
}
