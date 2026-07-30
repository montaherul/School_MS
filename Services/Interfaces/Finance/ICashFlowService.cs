using SchoolManagementSystem.Models.DTOs.Finance;

namespace SchoolManagementSystem.Services.Interfaces.Finance;

public interface ICashFlowService
{
    Task<CashFlowStatementDto> GetCashFlowStatementAsync(int year, int? month = null, int? periodType = 3, CancellationToken ct = default);
}
