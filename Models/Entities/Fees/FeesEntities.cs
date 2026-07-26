using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.Entities.Base;
using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Models.Entities.Fees;

public class FeeCategory : BaseEntity
{
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Description { get; set; }

    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}

public class FeeStructure : BaseEntity
{
    public int SchoolClassId { get; set; }
    public int? FeeCategoryId { get; set; }
    public int? AcademicYearId { get; set; }

    [MaxLength(100)]
    public string FeeName { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public decimal Amount { get; set; }
    public bool IsRecurring { get; set; }
    public FeeFrequency Frequency { get; set; } = FeeFrequency.Monthly;
    public int? DueDay { get; set; }
    public bool IsActive { get; set; } = true;
}

public class StudentFeeAssignment : BaseEntity
{
    public int StudentId { get; set; }
    public int FeeStructureId { get; set; }
    public int? AcademicYearId { get; set; }
    public decimal? CustomAmount { get; set; }
    public bool IsActive { get; set; } = true;
    public DateOnly? ValidFrom { get; set; }
    public DateOnly? ValidTo { get; set; }
}

public class FeeInvoice : BaseEntity
{
    [MaxLength(40)]
    public string InvoiceNo { get; set; } = string.Empty;

    public int StudentId { get; set; }
    public int? AcademicYearId { get; set; }
    public DateOnly DueDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal LateFee { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Draft;

    [MaxLength(500)]
    public string? Remarks { get; set; }
}

public class FeeInvoiceItem : BaseEntity
{
    public int FeeInvoiceId { get; set; }
    public int? FeeStructureId { get; set; }
    public int? FeeCategoryId { get; set; }

    [MaxLength(200)]
    public string Description { get; set; } = string.Empty;

    public decimal Amount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal NetAmount { get; set; }
}

public class Payment : BaseEntity
{
    public int FeeInvoiceId { get; set; }
    public FeeInvoice? FeeInvoice { get; set; }
    public decimal Amount { get; set; }
    public decimal LateFee { get; set; }
    public decimal DiscountAmount { get; set; }
    public PaymentMethod Method { get; set; }

    [MaxLength(80)]
    public string? ReferenceNo { get; set; }

    public DateTime PaidAt { get; set; } = DateTime.UtcNow;

    [MaxLength(500)]
    public string? Remarks { get; set; }
}

public class FeeDiscount : BaseEntity
{
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public FeeDiscountType DiscountType { get; set; } = FeeDiscountType.Percentage;
    public decimal Value { get; set; }
    public int? SchoolClassId { get; set; }
    public int? FeeCategoryId { get; set; }
    public int? FeeStructureId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateOnly? ValidFrom { get; set; }
    public DateOnly? ValidTo { get; set; }
}

public class FeeWaiver : BaseEntity
{
    public int StudentId { get; set; }
    public int? FeeInvoiceId { get; set; }
    public int? FeeCategoryId { get; set; }
    public int? FeeStructureId { get; set; }
    public FeeDiscountType WaiverType { get; set; } = FeeDiscountType.Percentage;
    public decimal WaiverValue { get; set; }
    public decimal WaiverAmount { get; set; }

    [MaxLength(500)]
    public string? Reason { get; set; }

    public bool IsApproved { get; set; }

    [MaxLength(64)]
    public string? ApprovedBy { get; set; }

    public DateTime? ApprovedAt { get; set; }

    [MaxLength(64)]
    public string? RejectedBy { get; set; }

    public DateTime? RejectedAt { get; set; }
    public DateOnly? ValidFrom { get; set; }
    public DateOnly? ValidTo { get; set; }
}

public class FeeRefund : BaseEntity
{
    public int FeePaymentId { get; set; }
    public decimal RefundAmount { get; set; }
    public PaymentMethod RefundMethod { get; set; }

    [MaxLength(200)]
    public string? ReferenceNo { get; set; }

    [MaxLength(500)]
    public string? Reason { get; set; }

    public bool IsApproved { get; set; }

    [MaxLength(64)]
    public string? ApprovedBy { get; set; }

    public DateTime? ApprovedAt { get; set; }

    [MaxLength(64)]
    public string? RejectedBy { get; set; }

    public DateTime? RejectedAt { get; set; }
    public DateTime RefundDate { get; set; } = DateTime.UtcNow;
}

[Index(nameof(StudentId))]
[Index(nameof(FeeInvoiceId))]
[Index(nameof(FeePaymentId))]
[Index(nameof(FeeDiscountId))]
[Index(nameof(FeeWaiverId))]
[Index(nameof(FeeRefundId))]
public class FeeLedger : BaseEntity
{
    public int StudentId { get; set; }
    public int? FeeInvoiceId { get; set; }
    public int? FeePaymentId { get; set; }
    public int? FeeDiscountId { get; set; }
    public int? FeeWaiverId { get; set; }
    public int? FeeRefundId { get; set; }
    public FeeLedgerType TransactionType { get; set; }
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
    public decimal Balance { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

    public FeeInvoice? FeeInvoice { get; set; }
    public Payment? FeePayment { get; set; }
    public FeeDiscount? FeeDiscount { get; set; }
    public FeeWaiver? FeeWaiver { get; set; }
    public FeeRefund? FeeRefund { get; set; }
}

public class FeeCollectionSummary : BaseEntity
{
    public DateOnly CollectionDate { get; set; }
    public decimal TotalCollected { get; set; }
    public decimal TotalDiscounted { get; set; }
    public decimal TotalRefunded { get; set; }
    public int TotalTransactions { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
    public bool IsDailySummary { get; set; } = true;
}

public class LateFeeRule : BaseEntity
{
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public int GraceDays { get; set; }
    public FeeDiscountType FeeType { get; set; } = FeeDiscountType.Fixed;
    public decimal FeeValue { get; set; }
    public decimal MaxFee { get; set; }
    public int? SchoolClassId { get; set; }
    public int? FeeCategoryId { get; set; }
    public bool IsActive { get; set; } = true;
}

public class FineRule : BaseEntity
{
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public int GraceDays { get; set; }
    public decimal FinePerDay { get; set; }
}

[Index(nameof(GatewayTransactionId), IsUnique = true, Name = "IX_OnlinePaymentRequests_GatewayTransactionId")]
public class OnlinePaymentRequest : BaseEntity
{
    public int StudentId { get; set; }
    public int FeeInvoiceId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    
    [MaxLength(80)]
    public string? ReferenceNo { get; set; }
    
    [MaxLength(500)]
    public string? Remarks { get; set; }
    
    public OnlinePaymentRequestStatus Status { get; set; } = OnlinePaymentRequestStatus.Pending;
    
    [MaxLength(64)]
    public string? VerifiedBy { get; set; }
    
    public DateTime? VerifiedAt { get; set; }
    
    [MaxLength(64)]
    public string? RejectedBy { get; set; }
    
    public DateTime? RejectedAt { get; set; }
    
    [MaxLength(500)]
    public string? AdminNotes { get; set; }

    [MaxLength(100)]
    public string? GatewayTransactionId { get; set; }

    [MaxLength(255)]
    public string? GatewaySessionKey { get; set; }

    [MaxLength(4000)]
    public string? GatewayResponse { get; set; }

    public DateTime? PaymentExpiryAt { get; set; }

    public PaymentPurpose PaymentPurpose { get; set; } = PaymentPurpose.StudentFee;

    public int? AdmissionApplicationId { get; set; }

    public FeeInvoice? FeeInvoice { get; set; }
}

public class AdmissionReceipt : BaseEntity
{
    [MaxLength(40)]
    public string ReceiptNo { get; set; } = string.Empty;

    public int AdmissionApplicationId { get; set; }

    public decimal Amount { get; set; }

    [MaxLength(50)]
    public string? PaymentMethod { get; set; }

    [MaxLength(100)]
    public string? GatewayTransactionId { get; set; }

    [MaxLength(200)]
    public string? ApplicantName { get; set; }

    public DateTime ReceiptDate { get; set; } = DateTime.UtcNow;

    [MaxLength(500)]
    public string? Remarks { get; set; }

    public bool IsRefunded { get; set; }

    public decimal? RefundAmount { get; set; }

    public DateTime? RefundedAt { get; set; }

    [MaxLength(64)]
    public string? RefundedBy { get; set; }

    [MaxLength(500)]
    public string? RefundReason { get; set; }

    public int? ConvertedStudentId { get; set; }
}

public class PaymentGatewayTransaction : BaseEntity
{
    public int OnlinePaymentRequestId { get; set; }
    public OnlinePaymentRequest? OnlinePaymentRequest { get; set; }

    [MaxLength(100)]
    public string GatewayName { get; set; } = "SSLCommerz";

    [MaxLength(100)]
    public string? GatewayTransactionId { get; set; }

    [MaxLength(100)]
    public string? BankTransactionId { get; set; }

    [MaxLength(100)]
    public string? ValidationId { get; set; }

    [MaxLength(20)]
    public string? CardType { get; set; }

    [MaxLength(10)]
    public string? Currency { get; set; }

    public decimal? GatewayAmount { get; set; }

    [MaxLength(20)]
    public string? GatewayStatus { get; set; }

    [MaxLength(20)]
    public string? RiskLevel { get; set; }

    public int AttemptCount { get; set; } = 1;

    [MaxLength(8000)]
    public string? InitRequestPayload { get; set; }

    [MaxLength(8000)]
    public string? InitResponsePayload { get; set; }

    [MaxLength(8000)]
    public string? IpnPayload { get; set; }

    [MaxLength(8000)]
    public string? ValidationPayload { get; set; }

    public DateTime? InitiatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}
