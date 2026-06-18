using System.Security.Claims;

namespace SchoolManagementSystem.Services.Interfaces.Fees;

public interface IFeeSecurityService
{
    bool Can(ClaimsPrincipal user, string permissionCode);
    bool HasStudentRole(ClaimsPrincipal user);
    int? GetCurrentStudentId(ClaimsPrincipal user);
    bool IsStudentScope(ClaimsPrincipal user, int studentId);
    bool CanAccessStudentData(ClaimsPrincipal user, int studentId);
    Task<bool> CanAccessInvoiceAsync(ClaimsPrincipal user, int invoiceId, CancellationToken ct = default);
    Task<bool> CanAccessPaymentAsync(ClaimsPrincipal user, int paymentId, CancellationToken ct = default);
    Task<bool> CanAccessRefundAsync(ClaimsPrincipal user, int refundId, CancellationToken ct = default);
}
