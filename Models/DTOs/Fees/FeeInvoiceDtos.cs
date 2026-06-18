using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models.DTOs.Fees;

public class FeeInvoiceListItemDto
{
    public int Id { get; set; }
    public string InvoiceNo { get; set; } = string.Empty;
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int? AcademicYearId { get; set; }
    public string? AcademicYearName { get; set; }
    public DateOnly DueDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal LateFee { get; set; }
    public int Status { get; set; }
    public string? Remarks { get; set; }
    public int TotalRecords { get; set; }
}

public class FeeInvoiceUpsertDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(40)]
    public string InvoiceNo { get; set; } = string.Empty;

    [Required]
    public int StudentId { get; set; }

    public int? AcademicYearId { get; set; }

    [Required]
    public DateOnly DueDate { get; set; }

    [Required]
    public decimal TotalAmount { get; set; }

    public decimal PaidAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal LateFee { get; set; }
    public int Status { get; set; } = 1;

    [StringLength(500)]
    public string? Remarks { get; set; }
}
