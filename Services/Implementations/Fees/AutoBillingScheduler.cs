using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Models.Entities.Student;
using SchoolManagementSystem.Services.Interfaces.Fees;
using SchoolManagementSystem.Services.Interfaces.Guardian;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Fees;

/// <summary>
/// Background worker that triggers monthly invoice generation for the active
/// academic year and notifies guardians of the newly generated invoices.
/// The underlying stored procedure (sp_GenerateMonthlyInvoices) is idempotent
/// per calendar month, and this worker adds a monthly guard so billing (and the
/// resulting notifications) run at most once per month per process.
/// </summary>
public class AutoBillingScheduler : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AutoBillingScheduler> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(6);

    private int? _lastBillingYear;
    private int? _lastBillingMonth;

    public AutoBillingScheduler(IServiceScopeFactory scopeFactory, ILogger<AutoBillingScheduler> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("AutoBillingScheduler started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunBillingIfDueAsync(stoppingToken);
            }
            catch (TaskCanceledException) when (stoppingToken.IsCancellationRequested)
            {
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AutoBillingScheduler encountered an error.");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }

        _logger.LogInformation("AutoBillingScheduler stopping.");
    }

    private async Task RunBillingIfDueAsync(CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        if (_lastBillingYear == now.Year && _lastBillingMonth == now.Month)
            return;

        using var scope = _scopeFactory.CreateScope();
        var autoBilling = scope.ServiceProvider.GetRequiredService<IAutoBillingService>();
        var guardianService = scope.ServiceProvider.GetRequiredService<IGuardianService>();
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var activeYear = await uow.Repository<AcademicYear>().Query()
            .Where(y => y.IsActive && !y.IsDeleted)
            .OrderByDescending(y => y.IsCurrent ? 1 : 0)
            .ThenByDescending(y => y.StartsOn)
            .FirstOrDefaultAsync(ct);

        if (activeYear is null)
        {
            _logger.LogWarning("AutoBillingScheduler: no active academic year found; skipping billing.");
            return;
        }

        var result = await autoBilling.GenerateMonthlyInvoicesAsync(activeYear.Id, 10, ct);

        if (!result.Success)
        {
            _logger.LogError("AutoBillingScheduler: billing failed for academic year {YearId}: {Error}",
                activeYear.Id, result.ErrorMessage);
            return;
        }

        _lastBillingYear = now.Year;
        _lastBillingMonth = now.Month;
        _logger.LogInformation(
            "AutoBillingScheduler: generated {Count} invoices for academic year {YearId} ({Name}).",
            result.InvoicesGenerated, activeYear.Id, activeYear.Name);

        if (result.InvoicesGenerated > 0)
            await NotifyGuardiansOfNewInvoicesAsync(uow, guardianService, activeYear.Id, now, ct);
    }

    private async Task NotifyGuardiansOfNewInvoicesAsync(
        IUnitOfWork uow, IGuardianService guardianService, int academicYearId, DateTime now, CancellationToken ct)
    {
        var billedInvoices = await uow.Repository<FeeInvoice>().Query()
            .Where(i => !i.IsDeleted
                && i.AcademicYearId == academicYearId
                && i.CreatedAt.Year == now.Year
                && i.CreatedAt.Month == now.Month)
            .ToListAsync(ct);

        if (billedInvoices.Count == 0) return;

        var studentIds = billedInvoices.Select(i => i.StudentId).Distinct().ToList();
        var students = await uow.Repository<Student>().Query()
            .Where(s => studentIds.Contains(s.Id) && !s.IsDeleted)
            .Select(s => new { s.Id, s.FullName })
            .ToListAsync(ct);

        var nameById = students.ToDictionary(s => s.Id, s => s.FullName);
        var notified = 0;

        foreach (var invoice in billedInvoices)
        {
            if (nameById.TryGetValue(invoice.StudentId, out var studentName))
            {
                await guardianService.CreateFeeDueNotificationAsync(invoice.StudentId, studentName, invoice.TotalAmount, ct);
                notified++;
            }
        }

        if (notified > 0)
            _logger.LogInformation("AutoBillingScheduler: queued fee-due notifications for {Count} students.", notified);
    }
}
