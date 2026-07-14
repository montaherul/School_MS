using SchoolManagementSystem.Models.DTOs.Accounting;
using SchoolManagementSystem.Models.DTOs.Common;

namespace SchoolManagementSystem.Services.Interfaces.Accounting;

public interface ILedgerService
{
    Task<PagedResult<GeneralLedgerEntryDto>> GetGeneralLedgerAsync(int? accountId, DateTime? from, DateTime? to, int? periodId, int page, int pageSize, CancellationToken ct = default);
    Task<TrialBalanceResultDto> GetTrialBalanceAsync(DateTime? asOfDate, int? periodId, CancellationToken ct = default);
    Task<IncomeStatementDto> GetIncomeStatementAsync(DateTime from, DateTime to, int? periodId, CancellationToken ct = default);
    Task<BalanceSheetDto> GetBalanceSheetAsync(DateTime? asOfDate, int? periodId, CancellationToken ct = default);
    Task<List<MonthlyIncomeSummaryDto>> GetMonthlyIncomeSummaryAsync(int year, int? periodId, CancellationToken ct = default);
}
