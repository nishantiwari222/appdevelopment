using Microsoft.Extensions.Logging;
using YourOwnJournal.Data;

namespace YourOwnJournal.Services;

public class AppInitializer
{
    private readonly DatabaseInitializer _databaseInitializer;
    private readonly SecurityService _securityService;
    private readonly AppState _appState;
    private readonly ILogger<AppInitializer> _logger;

    public AppInitializer(
        DatabaseInitializer databaseInitializer,
        SecurityService securityService,
        AppState appState,
        ILogger<AppInitializer> logger)
    {
        _databaseInitializer = databaseInitializer;
        _securityService = securityService;
        _appState = appState;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        if (_appState.IsInitialized || _appState.IsInitializing)
        {
            return;
        }

        _appState.SetInitializing(true);
        StartupLogger.Log("AppInitializer started.");

        try
        {
            await _databaseInitializer.InitializeAsync();
            await _securityService.IsPinSetAsync();
            _appState.SetLocked(true);
            _appState.MarkInitialized();
            StartupLogger.Log("AppInitializer completed.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "App initialization failed.");
            StartupLogger.Log($"AppInitializer error: {ex}");
            _appState.SetStartupError("Initialization failed. Check logs and restart the app.");
        }
    }
}
