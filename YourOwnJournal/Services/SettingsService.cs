using YourOwnJournal.Repositories;

namespace YourOwnJournal.Services;

public class SettingsService
{
    public const string ThemeKey = "Theme";
    public const string PageSizeKey = "PageSize";

    private readonly ISettingsRepository _settingsRepository;

    public SettingsService(ISettingsRepository settingsRepository)
    {
        _settingsRepository = settingsRepository;
    }

    public async Task<string> GetThemeAsync()
    {
        return await _settingsRepository.GetValueAsync(ThemeKey) ?? "light";
    }

    public Task SetThemeAsync(string theme) => _settingsRepository.SetValueAsync(ThemeKey, theme);

    public async Task<int> GetPageSizeAsync()
    {
        var value = await _settingsRepository.GetValueAsync(PageSizeKey);
        return int.TryParse(value, out var pageSize) ? pageSize : 10;
    }

    public Task SetPageSizeAsync(int pageSize) => _settingsRepository.SetValueAsync(PageSizeKey, pageSize.ToString());
}
