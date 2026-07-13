namespace SchoolManagementSystem.Models.DTOs.Fees;

public class StudentFeeProfileDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = "";
    public string StudentCode { get; set; } = "";
    public string ClassName { get; set; } = "";
    public string SectionName { get; set; } = "";
    public string AcademicYear { get; set; } = "";
    public string GuardianName { get; set; } = "";
    public string GuardianPhone { get; set; } = "";

    public decimal TotalAssigned { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal TotalDue { get; set; }
    public decimal TotalDiscount { get; set; }
    public decimal TotalWaiver { get; set; }
    public decimal TotalLateFee { get; set; }
    public int InvoiceCount { get; set; }
    public int PaymentCount { get; set; }
    public int OverdueCount { get; set; }

    public List<StudentFeeStructureInfoDto> FeeStructures { get; set; } = [];
    public List<FeeInvoiceListItemDto> Invoices { get; set; } = [];
    public List<FeePaymentListItemDto> Payments { get; set; } = [];
    public List<FeeLedgerListItemDto> LedgerEntries { get; set; } = [];
    public List<FeeDiscountListItemDto> Discounts { get; set; } = [];
    public List<FeeWaiverListItemDto> Waivers { get; set; } = [];
}

public class StudentFeeStructureInfoDto
{
    public int Id { get; set; }
    public string FeeStructureName { get; set; } = "";
    public string FeeCategoryName { get; set; } = "";
    public decimal Amount { get; set; }
    public decimal? CustomAmount { get; set; }
    public string Frequency { get; set; } = "";
    public bool IsActive { get; set; }
    public DateOnly? ValidFrom { get; set; }
    public DateOnly? ValidTo { get; set; }
}
