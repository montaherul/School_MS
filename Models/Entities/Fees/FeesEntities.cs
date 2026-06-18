using System.ComponentModel.DataAnnotations;
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
    public PaymentStatus Status { get; set; } = PaymentStatus.Unpaid;

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

public class FeeLedger : BaseEntity
{
    public int StudentId { get; set; }
    public int? FeeInvoiceId { get; set; }
    public int? FeePaymentId { get; set; }
    public FeeLedgerType TransactionType { get; set; }
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
    public decimal Balance { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
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
