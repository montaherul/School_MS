using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Services.Interfaces.Fees;
using SchoolManagementSystem.Services.Interfaces.Students;
using System.Security.Claims;

namespace SchoolManagementSystem.Services.Implementations.Fees;

public class FeeSecurityService : IFeeSecurityService
{
    private readonly IStudentService _studentService;
    private readonly IUnitOfWork _unitOfWork;

    public FeeSecurityService(IStudentService studentService, IUnitOfWork unitOfWork)
    {
        _studentService = studentService;
        _unitOfWork = unitOfWork;
    }

    public bool Can(ClaimsPrincipal user, string permissionCode)
        => user.IsInRole("Super Admin") || user.HasClaim("Permission", permissionCode);

    public bool HasStudentRole(ClaimsPrincipal user)
        => user.IsInRole("Student");

    public int? GetCurrentStudentId(ClaimsPrincipal user)
    {
        if (!int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return null;
        return _studentService.GetStudentIdByUserIdAsync(userId).GetAwaiter().GetResult();
    }

    public bool IsStudentScope(ClaimsPrincipal user, int studentId)
    {
        if (user.IsInRole("Super Admin") || user.IsInRole("Admin") || user.IsInRole("Accountant"))
            return true;
        return HasStudentRole(user) && GetCurrentStudentId(user) == studentId;
    }

    public bool CanAccessStudentData(ClaimsPrincipal user, int studentId)
        => IsStudentScope(user, studentId);

    public async Task<bool> CanAccessInvoiceAsync(ClaimsPrincipal user, int invoiceId, CancellationToken ct = default)
    {
        var invoice = await _unitOfWork.Repository<FeeInvoice>().GetByIdAsync(invoiceId, ct);
        if (invoice == null || invoice.IsDeleted) return false;
        return IsStudentScope(user, invoice.StudentId);
    }

    public async Task<bool> CanAccessPaymentAsync(ClaimsPrincipal user, int paymentId, CancellationToken ct = default)
    {
        var payment = await _unitOfWork.Repository<Payment>().GetByIdAsync(paymentId, ct);
        if (payment == null || payment.IsDeleted) return false;
        var invoice = await _unitOfWork.Repository<FeeInvoice>().GetByIdAsync(payment.FeeInvoiceId, ct);
        if (invoice == null || invoice.IsDeleted) return false;
        return IsStudentScope(user, invoice.StudentId);
    }

    public async Task<bool> CanAccessRefundAsync(ClaimsPrincipal user, int refundId, CancellationToken ct = default)
    {
        var refund = await _unitOfWork.Repository<FeeRefund>().GetByIdAsync(refundId, ct);
        if (refund == null || refund.IsDeleted) return false;
        var payment = await _unitOfWork.Repository<Payment>().GetByIdAsync(refund.FeePaymentId, ct);
        if (payment == null || payment.IsDeleted) return false;
        var invoice = await _unitOfWork.Repository<FeeInvoice>().GetByIdAsync(payment.FeeInvoiceId, ct);
        if (invoice == null || invoice.IsDeleted) return false;
        return IsStudentScope(user, invoice.StudentId);
    }
}
