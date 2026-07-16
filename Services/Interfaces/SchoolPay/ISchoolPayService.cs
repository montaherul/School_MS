using SchoolManagementSystem.Models.DTOs.SchoolPay;

namespace SchoolManagementSystem.Services.Interfaces.SchoolPay;

public interface ISchoolPayService
{
    Task<List<SchoolPayTransactionDto>> GetTransactionsPagedAsync(int page, int pageSize, string? status = null, string? providerCode = null, CancellationToken ct = default);
    Task<int> GetTransactionCountAsync(string? status = null, string? providerCode = null, CancellationToken ct = default);
    Task<SchoolPayDashboardDto> GetDashboardDataAsync(CancellationToken ct = default);
    Task<bool> LogAuditEventAsync(int? transactionId, string eventType, string? eventData, string? performedBy, string? ipAddress, CancellationToken ct = default);
}
