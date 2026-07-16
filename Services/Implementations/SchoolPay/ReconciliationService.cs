using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Models.DTOs.SchoolPay;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.SchoolPay;
using SchoolManagementSystem.Services.Interfaces.SchoolPay;

namespace SchoolManagementSystem.Services.Implementations.SchoolPay;

public class ReconciliationService : IReconciliationService
{
    private readonly ISchoolPayRepository _repo;
    private readonly ILogger<ReconciliationService> _logger;

    public ReconciliationService(ISchoolPayRepository repo, ILogger<ReconciliationService> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<List<SchoolPayReconciliationResultDto>> GetReconciliationForSettlementAsync(int settlementId, CancellationToken ct = default)
        => await _repo.GetReconciliationDataAsync(settlementId, ct);

    public async Task<SchoolPayReconciliationResultDto?> RunReconciliationAsync(int settlementId, CancellationToken ct = default)
    {
        _logger.LogInformation("Running reconciliation for settlement {SettlementId}", settlementId);

        var results = await _repo.GetReconciliationDataAsync(settlementId, ct);
        var result = results.FirstOrDefault();

        if (result != null)
        {
            var settlement = await _repo.GetSettlementEntityByIdAsync(settlementId, ct);
            if (settlement != null)
            {
                if (Math.Abs(result.Difference) < 0.01m)
                {
                    settlement.Status = SettlementStatus.Settled;
                    settlement.Remarks = $"Reconciled on {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC. Difference: {result.Difference:C}";
                }
                else
                {
                    settlement.Status = SettlementStatus.Disputed;
                    settlement.Remarks = $"Reconciliation mismatch on {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC. Difference: {result.Difference:C}";
                }
                await _repo.UpdateSettlementAsync(settlement, ct);
            }
        }

        return result;
    }

    public async Task RunBulkReconciliationAsync(CancellationToken ct = default)
    {
        var settlements = await _repo.GetSettlementsAsync(ct);
        var pendingSettlements = settlements
            .Where(s => s.Status == SettlementStatus.Pending || s.Status == SettlementStatus.Processing)
            .ToList();

        _logger.LogInformation("Running bulk reconciliation for {Count} settlements", pendingSettlements.Count);

        foreach (var s in pendingSettlements)
        {
            if (ct.IsCancellationRequested) break;
            try
            {
                await RunReconciliationAsync(s.Id, ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Reconciliation error for settlement {SettlementId}", s.Id);
            }
        }
    }
}
