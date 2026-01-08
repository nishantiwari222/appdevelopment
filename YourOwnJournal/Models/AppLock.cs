using System.ComponentModel.DataAnnotations;

namespace YourOwnJournal.Models;

public class AppLock
{
    [Key]
    [MaxLength(50)]
    public string Key { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Value { get; set; } = string.Empty;
}
