using YourOwnJournal.Models;
using YourOwnJournal.Repositories;

namespace YourOwnJournal.Services;

public class TagService
{
    private readonly ITagRepository _tagRepository;

    public TagService(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }

    public Task<IReadOnlyList<Tag>> GetAllAsync() => _tagRepository.GetAllAsync();
}
