using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Models.DTOs.SchoolPay;
using SchoolManagementSystem.Repositories.Interfaces.SchoolPay;
using SchoolManagementSystem.Services.Interfaces.SchoolPay;

namespace SchoolManagementSystem.Services.Implementations.SchoolPay;

public class SchoolPayService : ISchoolPayService
{
    private readonly ISchoolPayRepository _repository;
    private readonly ILogger<SchoolPayService> _logger;

    public SchoolPayService(ISchoolPayRepository repository, ILogger<SchoolPayService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public Task<List<SchoolPayTransactionDto>> GetTransactionsPagedAsync(int page, int pageSize, string? status = null, string? providerCode = null, CancellationToken ct = default)
        => _repository.GetTransactionsPagedAsync(page, pageSize, status, providerCode, ct);

    public Task<int> GetTransactionCountAsync(string? status = null, string? providerCode = null, CancellationToken ct = default)
        => _repository.GetTransactionCountAsync(status, providerCode, ct);

    public Task<SchoolPayDashboardDto> GetDashboardDataAsync(CancellationToken ct = default)
        => _repository.GetDashboardDataAsync(ct);

    public Task<bool> LogAuditEventAsync(int? transactionId, string eventType, string? eventData, string? performedBy, string? ipAddress, CancellationToken ct = default)
        => _repository.LogAuditEventAsync(transactionId, eventType, eventData, performedBy, ipAddress, ct);
}
