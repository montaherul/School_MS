using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Entities.Base;
using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Models.Entities.Fees;

public class FeeStructure : BaseEntity
{
    public int SchoolClassId { get; set; }

    [MaxLength(100)]
    public string FeeName { get; set; } = string.Empty;

    public decimal Amount { get; set; }
    public bool IsRecurring { get; set; }
}

public class FeeInvoice : BaseEntity
{
    [MaxLength(40)]
    public string InvoiceNo { get; set; } = string.Empty;

    public int StudentId { get; set; }
    public DateOnly DueDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Unpaid;
}

public class Payment : BaseEntity
{
    public int FeeInvoiceId { get; set; }
    public FeeInvoice? FeeInvoice { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }

    [MaxLength(80)]
    public string? ReferenceNo { get; set; }

    public DateTime PaidAt { get; set; } = DateTime.UtcNow;
}

public class FineRule : BaseEntity
{
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public int GraceDays { get; set; }
    public decimal FinePerDay { get; set; }
}
