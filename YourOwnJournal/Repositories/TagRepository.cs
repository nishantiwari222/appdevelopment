using Microsoft.EntityFrameworkCore;
using YourOwnJournal.Data;
using YourOwnJournal.Models;

namespace YourOwnJournal.Repositories;

public class TagRepository : ITagRepository
{
    private readonly AppDbContext _dbContext;

    public TagRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Tag>> GetAllAsync()
    {
        return await _dbContext.Tags
            .OrderBy(t => t.IsPrebuilt ? 0 : 1)
            .ThenBy(t => t.Name)
            .ToListAsync();
    }

    public async Task<Tag?> GetByNameAsync(string name)
    {
        return await _dbContext.Tags.FirstOrDefaultAsync(t => t.Name == name);
    }

    public async Task<Tag> AddAsync(Tag tag)
    {
        _dbContext.Tags.Add(tag);
        await _dbContext.SaveChangesAsync();
        return tag;
    }
}
