#if Dapper
using DbUp;
using DbUp.Engine;
using Microsoft.Data.SqlClient;
using System.Reflection;
#else
using Microsoft.EntityFrameworkCore;
#endif

namespace MinimalApiTemplate.REST.Persistance;

public class DatabaseStateManager
{
#if Dapper
    private readonly UpgradeEngine upgradeEngine;
    private readonly string connectionString;
#else
    private readonly ILogger<DatabaseStateManager> _logger;
    private readonly AppDbContext _dbContext;
#endif

#if Dapper
    public DatabaseStateManager(IConfiguration configuration)
    {
        connectionString = configuration.GetConnectionString("Default");
        upgradeEngine = DeployChanges.To
            .SqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
            .LogToAutodetectedLog()
            .Build();
    }
#else
    public DatabaseStateManager(ILogger<DatabaseStateManager> logger, AppDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }
#endif

#if Dapper
    public void ApplyMigrations()
    {
        var retryCounter = 0;

        while (true)
        {
            try
            {
                EnsureDatabase.For.SqlDatabase(connectionString);
                break;
            }
            catch (SqlException ex)
            {
                if (retryCounter > 10)
                {
                    throw new DatabaseException("Cannot connect to a database server.", ex);
                }

                retryCounter++;
                Thread.Sleep(1000);
            }
        }

        if (upgradeEngine.IsUpgradeRequired())
        {
            var dbUpgradeResult = upgradeEngine.PerformUpgrade();

            if (!dbUpgradeResult.Successful)
            {
                throw new DatabaseException("Error while applying migrations.", dbUpgradeResult.Error);
            }
        }
    }
#else
    public async Task ApplyMigrations()
    {
        try
        {
            var appliedMigrations = await _dbContext.Database.GetAppliedMigrationsAsync();
            var pendingMigrations = await _dbContext.Database.GetPendingMigrationsAsync();

            if (pendingMigrations.Any())
            {
                await _dbContext.Database.MigrateAsync();
            }
            else if (appliedMigrations.ToList().Count == 0)
            {
                _logger.LogWarning("Application is starting without any migrations applied.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while initializig database");
            throw;
        }
    }
#endif
}