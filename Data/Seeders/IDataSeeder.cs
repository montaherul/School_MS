using SchoolManagementSystem.Models.Entities.Auth;
using SchoolManagementSystem.Models.Entities.Employee;

namespace SchoolManagementSystem.Data.Seeders;

/// <summary>
/// Contract for all data seeders. Register and call via IDataSeederRunner.
/// </summary>
public interface IDataSeeder
{
    /// <summary>Execution order. Lower numbers run first.</summary>
    int Order { get; }
    string Name { get; }
    Task SeedAsync(CancellationToken ct = default);
}

/// <summary>
/// Runs all registered IDataSeeder implementations in Order sequence.
/// Register in DI, call once at application startup.
/// </summary>
public interface IDataSeederRunner
{
    Task RunAllAsync(CancellationToken ct = default);
}

public class DataSeederRunner : IDataSeederRunner
{
    private readonly IEnumerable<IDataSeeder> _seeders;
    private readonly ILogger<DataSeederRunner> _logger;

    public DataSeederRunner(IEnumerable<IDataSeeder> seeders, ILogger<DataSeederRunner> logger)
    {
        _seeders = seeders;
        _logger  = logger;
    }

    public async Task RunAllAsync(CancellationToken ct = default)
    {
        foreach (var seeder in _seeders.OrderBy(s => s.Order))
        {
            try
            {
                _logger.LogInformation("Running seeder: {Name}", seeder.Name);
                await seeder.SeedAsync(ct);
                _logger.LogInformation("Completed seeder: {Name}", seeder.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Seeder {Name} failed", seeder.Name);
                throw; // fail fast — data integrity matters
            }
        }
    }
}
