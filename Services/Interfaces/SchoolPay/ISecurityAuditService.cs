using SchoolManagementSystem.Models.DTOs.SchoolPay;
using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Services.Interfaces.SchoolPay;

public interface ISecurityAuditService
{
    Task<List<SchoolPaySecurityAuditEntryDto>> GetAuditLogAsync(int? providerId = null, int days = 30, CancellationToken ct = default);
    Task LogSecurityEventAsync(PaymentSecurityEventType eventType, string? details, string? performedBy, string? ipAddress, CancellationToken ct = default);
}
