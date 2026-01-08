using System.ComponentModel.DataAnnotations;

namespace YourOwnJournal.Models;

public class JournalEntryEditModel
{
    public int? EntryId { get; set; }

    public DateTime EntryDate { get; set; } = DateTime.Today;

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string ContentMarkdown { get; set; } = string.Empty;

    [Required]
    public int? PrimaryMoodId { get; set; }

    public List<int> SecondaryMoodIds { get; set; } = new();

    public List<string> Tags { get; set; } = new();
}
