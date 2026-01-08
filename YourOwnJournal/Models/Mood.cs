using System.ComponentModel.DataAnnotations;

namespace YourOwnJournal.Models;

public class Mood
{
    public int MoodId { get; set; }

    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(20)]
    public string Category { get; set; } = string.Empty;

    public ICollection<JournalEntry> PrimaryEntries { get; set; } = new List<JournalEntry>();

    public ICollection<EntrySecondaryMood> SecondaryEntries { get; set; } = new List<EntrySecondaryMood>();
}
