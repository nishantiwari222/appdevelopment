using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace YourOwnJournal.Data;

public class DatabaseInitializer
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<DatabaseInitializer> _logger;

    public DatabaseInitializer(AppDbContext dbContext, ILogger<DatabaseInitializer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        try
        {
            await _dbContext.Database.EnsureCreatedAsync();
            if (!await TableExistsAsync("AppLocks"))
            {
                _logger.LogWarning("AppLocks table missing. Recreating database.");
                await RecreateDatabaseAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Database migration failed, falling back to EnsureCreated.");
            await RecreateDatabaseAsync();
        }
    }

    private async Task RecreateDatabaseAsync()
    {
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.Database.EnsureCreatedAsync();
    }

    private async Task<bool> TableExistsAsync(string tableName)
    {
        await using var connection = _dbContext.Database.GetDbConnection();
        if (connection.State != System.Data.ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name=$name;";
        var param = command.CreateParameter();
        param.ParameterName = "$name";
        param.Value = tableName;
        command.Parameters.Add(param);
        var result = await command.ExecuteScalarAsync();
        return result != null && result != DBNull.Value;
    }
}
