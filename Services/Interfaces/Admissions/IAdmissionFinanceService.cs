using SchoolManagementSystem.Models.DTOs.Admission;

namespace SchoolManagementSystem.Services.Interfaces.Admissions;

public interface IAdmissionFinanceService
{
    Task<List<AdmissionFeeSummaryListItemDto>> GetAllFeeSummariesAsync(CancellationToken ct = default);
    Task<AdmissionFeeSummaryDto> GetFeeSummaryAsync(int applicationId, CancellationToken ct = default);
    Task<AdmissionPaymentHistoryDto> RecordPaymentAsync(AdmissionFeePaymentRequest request, string receivedBy, CancellationToken ct = default);
    Task<bool> ApplyScholarshipAsync(int applicationId, decimal percentage, string? description, string appliedBy, CancellationToken ct = default);
    Task<bool> ApplyWaiverAsync(int applicationId, decimal amount, string? description, string appliedBy, CancellationToken ct = default);
    Task<List<AdmissionInstallmentPlanDto>> CreateInstallmentPlanAsync(int applicationId, int installments, string createdBy, CancellationToken ct = default);
    Task<bool> ProcessRefundAsync(int applicationId, decimal amount, string reason, string processedBy, CancellationToken ct = default);
    Task<int> CreateAdmissionInvoiceAsync(int applicationId, int studentId, decimal admissionFee, bool isPaid, string? className, string? paymentMethod, string? transactionDetails, string createdBy, CancellationToken ct = default);
}
