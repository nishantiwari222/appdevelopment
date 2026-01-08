namespace YourOwnJournal.Services;

public record MoodCount(string Name, int Count);
public record CategoryCount(string Category, int Count);
public record TagCount(string Name, int Count);
public record WordCountPoint(DateTime Date, int WordCount);
