using SchoolManagementSystem.Models.DTOs.Finance;

namespace SchoolManagementSystem.Repositories.Interfaces.Finance;

public interface ICashFlowRepository
{
    Task<CashFlowStatementDto> GetCashFlowStatementAsync(int year, int? month = null, int? periodType = 3, CancellationToken ct = default);
}
