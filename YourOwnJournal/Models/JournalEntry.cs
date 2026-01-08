using System.ComponentModel.DataAnnotations;

namespace YourOwnJournal.Models;

public class JournalEntry
{
    [Key]
    public int EntryId { get; set; }

    [Required]
    public DateTime EntryDate { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string ContentMarkdown { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public int PrimaryMoodId { get; set; }
    public Mood? PrimaryMood { get; set; }

    public ICollection<EntrySecondaryMood> SecondaryMoods { get; set; } = new List<EntrySecondaryMood>();

    public ICollection<EntryTag> EntryTags { get; set; } = new List<EntryTag>();
}
