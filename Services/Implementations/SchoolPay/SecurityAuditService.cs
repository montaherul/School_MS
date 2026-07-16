using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Models.DTOs.SchoolPay;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.SchoolPay;
using SchoolManagementSystem.Services.Interfaces.SchoolPay;

namespace SchoolManagementSystem.Services.Implementations.SchoolPay;

public class SecurityAuditService : ISecurityAuditService
{
    private readonly ISchoolPayRepository _repo;
    private readonly ILogger<SecurityAuditService> _logger;

    public SecurityAuditService(ISchoolPayRepository repo, ILogger<SecurityAuditService> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<List<SchoolPaySecurityAuditEntryDto>> GetAuditLogAsync(int? providerId = null, int days = 30, CancellationToken ct = default)
        => await _repo.GetSecurityAuditLogAsync(providerId, days, ct);

    public async Task LogSecurityEventAsync(PaymentSecurityEventType eventType, string? details, string? performedBy, string? ipAddress, CancellationToken ct = default)
    {
        await _repo.LogSecurityEventAsync(eventType, details, performedBy, ipAddress, ct);
        _logger.LogInformation("Security event: {EventType} by {User}", eventType, performedBy);
    }
}
