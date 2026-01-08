namespace YourOwnJournal.Services;

public class AppState
{
    public bool IsLocked { get; private set; }
    public bool IsInitialized { get; private set; }
    public bool IsInitializing { get; private set; }
    public string? StartupError { get; private set; }
    public string Theme { get; private set; } = "light";

    public event Action? OnChange;

    public void SetLocked(bool locked)
    {
        IsLocked = locked;
        OnChange?.Invoke();
    }

    public void MarkInitialized()
    {
        IsInitialized = true;
        IsInitializing = false;
        OnChange?.Invoke();
    }

    public void SetInitializing(bool initializing)
    {
        IsInitializing = initializing;
        OnChange?.Invoke();
    }

    public void SetStartupError(string message)
    {
        StartupError = message;
        IsInitializing = false;
        OnChange?.Invoke();
    }

    public void SetTheme(string theme)
    {
        Theme = theme;
        OnChange?.Invoke();
    }
}
