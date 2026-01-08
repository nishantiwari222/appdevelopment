using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuestPDF.Infrastructure;
using YourOwnJournal.Data;
using YourOwnJournal.Repositories;
using YourOwnJournal.Services;

namespace YourOwnJournal;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		StartupLogger.Log("CreateMauiApp started.");
		try
		{
			AppDomain.CurrentDomain.UnhandledException += (_, args) =>
			{
				StartupLogger.Log($"UnhandledException: {args.ExceptionObject}");
			};
			TaskScheduler.UnobservedTaskException += (_, args) =>
			{
				StartupLogger.Log($"UnobservedTaskException: {args.Exception}");
				args.SetObserved();
			};

			var builder = MauiApp.CreateBuilder();
			StartupLogger.Log("MauiApp.CreateBuilder done.");
			builder
				.UseMauiApp<App>()
				.ConfigureFonts(fonts =>
				{
					fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				});
			StartupLogger.Log("UseMauiApp configured.");

			builder.Services.AddMauiBlazorWebView();
			StartupLogger.Log("AddMauiBlazorWebView done.");
			if (!OperatingSystem.IsMacCatalyst())
			{
				QuestPDF.Settings.License = LicenseType.Community;
				StartupLogger.Log("QuestPDF license set.");
			}
			else
			{
				StartupLogger.Log("QuestPDF skipped on MacCatalyst.");
			}

			SQLitePCL.Batteries_V2.Init();
			StartupLogger.Log("SQLitePCL initialized.");

			var dbPath = Path.Combine(FileSystem.AppDataDirectory, "yourownjournal.db");
			builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite($"Filename={dbPath}"));
			builder.Services.AddScoped<DatabaseInitializer>();

			builder.Services.AddScoped<IJournalEntryRepository, JournalEntryRepository>();
			builder.Services.AddScoped<IMoodRepository, MoodRepository>();
			builder.Services.AddScoped<ITagRepository, TagRepository>();
			builder.Services.AddScoped<ISettingsRepository, SettingsRepository>();
			builder.Services.AddScoped<IAppLockRepository, AppLockRepository>();

			builder.Services.AddScoped<JournalService>();
			builder.Services.AddScoped<MoodService>();
			builder.Services.AddScoped<TagService>();
			builder.Services.AddScoped<SettingsService>();
			builder.Services.AddScoped<SecurityService>();
			builder.Services.AddScoped<StreakService>();
			builder.Services.AddScoped<AnalyticsService>();
		builder.Services.AddScoped<ExportService>();
#if MACCATALYST
		builder.Services.AddSingleton<IPdfExportService, MacCatalystPdfExportService>();
#else
		builder.Services.AddSingleton<IPdfExportService, NoopPdfExportService>();
#endif
		builder.Services.AddScoped<AppInitializer>();
			builder.Services.AddSingleton<AppState>();

#if DEBUG
			builder.Services.AddBlazorWebViewDeveloperTools();
			builder.Logging.AddDebug();
#endif

			var app = builder.Build();
			StartupLogger.Log("MauiApp built.");
			return app;
		}
		catch (Exception ex)
		{
			StartupLogger.Log($"CreateMauiApp error: {ex}");
			throw;
		}
	}
}
