using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Models.DTOs.Accounting;

public class JournalEntryListItemDto
{
    public int Id { get; set; }
    public string JournalNo { get; set; } = string.Empty;
    public DateTime EntryDate { get; set; }
    public string EntryType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal TotalDebit { get; set; }
    public decimal TotalCredit { get; set; }
    public bool IsPosted { get; set; }
    public int TotalRecords { get; set; }
}

public class JournalEntryUpsertDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string JournalNo { get; set; } = string.Empty;

    public DateTime EntryDate { get; set; } = DateTime.Today;

    [Required]
    public JournalEntryType EntryType { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    public int? FinancialPeriodId { get; set; }

    public int? ReferenceId { get; set; }

    [StringLength(100)]
    public string? ReferenceType { get; set; }

    public List<JournalLineDto> Lines { get; set; } = [];
}

public class JournalLineDto
{
    public int AccountId { get; set; }

    [Required]
    public JournalLineType LineType { get; set; }

    [Required]
    [Range(0.01, 999999999)]
    public decimal Amount { get; set; }

    [StringLength(500)]
    public string? Narration { get; set; }
}

public class JournalEntryDetailDto
{
    public int Id { get; set; }
    public string JournalNo { get; set; } = string.Empty;
    public DateTime EntryDate { get; set; }
    public string EntryType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? FinancialPeriodId { get; set; }
    public string? FinancialPeriodName { get; set; }
    public bool IsPosted { get; set; }
    public DateTime? PostedAt { get; set; }
    public string? PostedBy { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<JournalEntryLineDetailDto> Lines { get; set; } = [];
    public decimal TotalDebit => Lines.Where(l => l.LineType == "Debit").Sum(l => l.Amount);
    public decimal TotalCredit => Lines.Where(l => l.LineType == "Credit").Sum(l => l.Amount);
}

public class JournalEntryLineDetailDto
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public string AccountCode { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string LineType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? Narration { get; set; }
}
