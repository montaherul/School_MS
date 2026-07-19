using SchoolManagementSystem.Models.Entities.Admission;
using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Models.DTOs.Admission;

public class AdmissionFeePaymentRequest
{
    public int ApplicationId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? Remarks { get; set; }
}

public class AdmissionPaymentHistoryDto
{
    public int Id { get; set; }
    public int ApplicationId { get; set; }
    public string ApplicationNo { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime PaidAt { get; set; }
    public string? Remarks { get; set; }
    public string? ReceivedBy { get; set; }
}

public class AdmissionFeeSummaryListItemDto
{
    public int ApplicationId { get; set; }
    public string ApplicationNo { get; set; } = string.Empty;
    public string ApplicantName { get; set; } = string.Empty;
    public string? AppliedClass { get; set; }
    public decimal AdmissionFee { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal DueAmount => AdmissionFee - PaidAmount;
    public bool IsPaid => DueAmount <= 0;
    public string PaymentStatus => IsPaid ? "Paid" : "Unpaid";
    public AdmissionStatus Status { get; set; }
    public DateTime AppliedAt { get; set; }
    public DateTime? LastPaymentAt { get; set; }
}

public class AdmissionFeeSummaryDto
{
    public int ApplicationId { get; set; }
    public string ApplicationNo { get; set; } = string.Empty;
    public string ApplicantName { get; set; } = string.Empty;
    public decimal AdmissionFee { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal DueAmount => AdmissionFee - PaidAmount;
    public bool IsPaid => DueAmount <= 0;
    public string PaymentStatus => IsPaid ? "Paid" : "Unpaid";
    public List<AdmissionPaymentHistoryDto> Payments { get; set; } = new();
}

public class AdmissionScholarshipDto
{
    public int ApplicationId { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal DiscountAmount { get; set; }
    public string? Description { get; set; }
    public bool IsWaiver { get; set; }
}

public class AdmissionInstallmentPlanDto
{
    public int Id { get; set; }
    public int ApplicationId { get; set; }
    public int InstallmentNumber { get; set; }
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
    public bool IsPaid { get; set; }
    public DateTime? PaidAt { get; set; }
}
