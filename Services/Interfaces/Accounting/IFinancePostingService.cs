namespace SchoolManagementSystem.Services.Interfaces.Accounting;

public interface IFinancePostingService
{
    Task<int> PostFeeCollectionAsync(int studentId, decimal amount, int invoiceId, int accountId, string createdBy, CancellationToken ct = default);
    Task<int> PostFeeCollectionAsync(int studentId, decimal amount, int invoiceId, string createdBy, CancellationToken ct = default);
    Task<int> PostFeeWaiverAsync(int studentId, decimal amount, string description, string createdBy, CancellationToken ct = default);
    Task<int> PostBankReceiptAsync(int accountId, decimal amount, string referenceNo, string description, string createdBy, CancellationToken ct = default);
    Task<int> PostBankPaymentAsync(int accountId, decimal amount, string referenceNo, string description, string createdBy, CancellationToken ct = default);
    Task<int> PostAdmissionFeeAsync(int admissionId, decimal amount, string paymentMethod, string gatewayTransactionId, string createdBy, CancellationToken ct = default);
    Task<int> PostAdmissionRefundAsync(int admissionId, decimal amount, string reason, string createdBy, CancellationToken ct = default);
}
