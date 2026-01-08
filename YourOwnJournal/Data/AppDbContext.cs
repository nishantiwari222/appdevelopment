using Microsoft.EntityFrameworkCore;
using YourOwnJournal.Models;

namespace YourOwnJournal.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<JournalEntry> JournalEntries => Set<JournalEntry>();
    public DbSet<Mood> Moods => Set<Mood>();
    public DbSet<EntrySecondaryMood> EntrySecondaryMoods => Set<EntrySecondaryMood>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<EntryTag> EntryTags => Set<EntryTag>();
    public DbSet<AppSetting> AppSettings => Set<AppSetting>();
    public DbSet<AppLock> AppLocks => Set<AppLock>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<JournalEntry>()
            .HasKey(e => e.EntryId);

        modelBuilder.Entity<JournalEntry>()
            .HasIndex(e => e.EntryDate)
            .IsUnique();

        modelBuilder.Entity<JournalEntry>()
            .HasIndex(e => e.Title);

        modelBuilder.Entity<Tag>()
            .HasIndex(t => t.Name)
            .IsUnique();

        modelBuilder.Entity<EntrySecondaryMood>()
            .HasKey(es => new { es.EntryId, es.MoodId });

        modelBuilder.Entity<EntryTag>()
            .HasKey(et => new { et.EntryId, et.TagId });

        modelBuilder.Entity<JournalEntry>()
            .HasOne(e => e.PrimaryMood)
            .WithMany(m => m.PrimaryEntries)
            .HasForeignKey(e => e.PrimaryMoodId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EntrySecondaryMood>()
            .HasOne(es => es.Entry)
            .WithMany(e => e.SecondaryMoods)
            .HasForeignKey(es => es.EntryId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<EntrySecondaryMood>()
            .HasOne(es => es.Mood)
            .WithMany(m => m.SecondaryEntries)
            .HasForeignKey(es => es.MoodId);

        modelBuilder.Entity<EntryTag>()
            .HasOne(et => et.Entry)
            .WithMany(e => e.EntryTags)
            .HasForeignKey(et => et.EntryId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<EntryTag>()
            .HasOne(et => et.Tag)
            .WithMany(t => t.EntryTags)
            .HasForeignKey(et => et.TagId);

        modelBuilder.Entity<Mood>().HasData(
            new Mood { MoodId = 1, Name = "Joyful", Category = "Positive" },
            new Mood { MoodId = 2, Name = "Grateful", Category = "Positive" },
            new Mood { MoodId = 3, Name = "Calm", Category = "Neutral" },
            new Mood { MoodId = 4, Name = "Focused", Category = "Neutral" },
            new Mood { MoodId = 5, Name = "Stressed", Category = "Negative" },
            new Mood { MoodId = 6, Name = "Sad", Category = "Negative" }
        );

        modelBuilder.Entity<Tag>().HasData(
            new Tag { TagId = 1, Name = "Work", IsPrebuilt = true },
            new Tag { TagId = 2, Name = "Health", IsPrebuilt = true },
            new Tag { TagId = 3, Name = "Family", IsPrebuilt = true },
            new Tag { TagId = 4, Name = "Travel", IsPrebuilt = true },
            new Tag { TagId = 5, Name = "Learning", IsPrebuilt = true },
            new Tag { TagId = 6, Name = "Reflection", IsPrebuilt = true }
        );
    }
}
