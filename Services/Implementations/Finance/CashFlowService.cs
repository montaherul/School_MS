using SchoolManagementSystem.Models.DTOs.Finance;
using SchoolManagementSystem.Repositories.Interfaces.Finance;
using SchoolManagementSystem.Services.Interfaces.Finance;

namespace SchoolManagementSystem.Services.Implementations.Finance;

public class CashFlowService : ICashFlowService
{
    private readonly ICashFlowRepository _repo;

    public CashFlowService(ICashFlowRepository repo) { _repo = repo; }

    public async Task<CashFlowStatementDto> GetCashFlowStatementAsync(int year, int? month = null, int? periodType = 3, CancellationToken ct = default)
        => await _repo.GetCashFlowStatementAsync(year, month, periodType, ct);
}
