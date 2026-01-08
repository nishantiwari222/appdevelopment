using System.Text;

namespace YourOwnJournal.Services;

public static class StartupLogger
{
    private static readonly object LockObj = new();

    public static string LogPath
    {
        get
        {
            try
            {
                return Path.Combine(FileSystem.AppDataDirectory, "startup.log");
            }
            catch
            {
                return Path.Combine(Path.GetTempPath(), "yourownjournal-startup.log");
            }
        }
    }

    public static void Log(string message)
    {
        try
        {
            lock (LockObj)
            {
                var line = $"[{DateTime.UtcNow:O}] {message}{Environment.NewLine}";
                File.AppendAllText(LogPath, line, Encoding.UTF8);
            }
        }
        catch
        {
        }
    }
}
