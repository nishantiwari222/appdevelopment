namespace YourOwnJournal.Models;

public class EntrySecondaryMood
{
    public int EntryId { get; set; }
    public JournalEntry? Entry { get; set; }

    public int MoodId { get; set; }
    public Mood? Mood { get; set; }
}
