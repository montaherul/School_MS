using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Services.Interfaces.Fees;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Fees;

public class OneTimeFeeScheduler : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OneTimeFeeScheduler> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(12);

    private DateOnly? _lastRunDate;

    public OneTimeFeeScheduler(IServiceScopeFactory scopeFactory, ILogger<OneTimeFeeScheduler> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("OneTimeFeeScheduler started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunOneTimeBillingIfDueAsync(stoppingToken);
            }
            catch (TaskCanceledException) when (stoppingToken.IsCancellationRequested) { }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OneTimeFeeScheduler encountered an error.");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }

        _logger.LogInformation("OneTimeFeeScheduler stopping.");
    }

    private async Task RunOneTimeBillingIfDueAsync(CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        if (_lastRunDate == today)
            return;

        using var scope = _scopeFactory.CreateScope();
        var autoBilling = scope.ServiceProvider.GetRequiredService<IAutoBillingService>();
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var activeYear = await uow.Repository<AcademicYear>().Query()
            .Where(y => y.IsActive && !y.IsDeleted)
            .OrderByDescending(y => y.IsCurrent ? 1 : 0)
            .ThenByDescending(y => y.StartsOn)
            .FirstOrDefaultAsync(ct);

        if (activeYear is null)
        {
            _logger.LogWarning("OneTimeFeeScheduler: no active academic year found.");
            return;
        }

        var result = await autoBilling.GenerateOneTimeFeeInvoicesAsync(activeYear.Id, ct: ct);

        _lastRunDate = today;

        if (result.InvoicesGenerated > 0)
            _logger.LogInformation("OneTimeFeeScheduler: generated {Count} one-time invoices for {Year}.",
                result.InvoicesGenerated, activeYear.Name);
    }
}
