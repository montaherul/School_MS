using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models.DTOs.Fees;

public class FeeWaiverListItemDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int? FeeInvoiceId { get; set; }
    public string? InvoiceNo { get; set; }
    public int? FeeCategoryId { get; set; }
    public string? FeeCategoryName { get; set; }
    public int? FeeStructureId { get; set; }
    public string? FeeStructureName { get; set; }
    public int WaiverType { get; set; }
    public decimal WaiverValue { get; set; }
    public decimal WaiverAmount { get; set; }
    public string? Reason { get; set; }
    public bool IsApproved { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? RejectedBy { get; set; }
    public DateTime? RejectedAt { get; set; }
    public DateOnly? ValidFrom { get; set; }
    public DateOnly? ValidTo { get; set; }
    public int TotalRecords { get; set; }
}

public class FeeWaiverUpsertDto
{
    public int Id { get; set; }

    [Required]
    public int StudentId { get; set; }

    public int? FeeInvoiceId { get; set; }
    public int? FeeCategoryId { get; set; }
    public int? FeeStructureId { get; set; }

    [Required]
    public int WaiverType { get; set; }

    [Required]
    public decimal WaiverValue { get; set; }

    public decimal WaiverAmount { get; set; }

    [StringLength(500)]
    public string? Reason { get; set; }

    public bool IsApproved { get; set; }
    public DateOnly? ValidFrom { get; set; }
    public DateOnly? ValidTo { get; set; }
}
