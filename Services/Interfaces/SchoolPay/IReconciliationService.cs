using SchoolManagementSystem.Models.DTOs.SchoolPay;

namespace SchoolManagementSystem.Services.Interfaces.SchoolPay;

public interface IReconciliationService
{
    Task<List<SchoolPayReconciliationResultDto>> GetReconciliationForSettlementAsync(int settlementId, CancellationToken ct = default);
    Task<SchoolPayReconciliationResultDto?> RunReconciliationAsync(int settlementId, CancellationToken ct = default);
    Task RunBulkReconciliationAsync(CancellationToken ct = default);
}
