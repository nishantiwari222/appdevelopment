using System.ComponentModel.DataAnnotations;

namespace YourOwnJournal.Models;

public class Tag
{
    public int TagId { get; set; }

    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    public bool IsPrebuilt { get; set; }

    public ICollection<EntryTag> EntryTags { get; set; } = new List<EntryTag>();
}
