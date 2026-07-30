using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Entities.Base;
using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Models.Entities.Accounting;

public class ChartOfAccount : BaseEntity
{
    [MaxLength(20)]
    public string AccountCode { get; set; } = string.Empty;

    [MaxLength(200)]
    public string AccountName { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public AccountType AccountType { get; set; }

    public int? ParentAccountId { get; set; }

    public bool IsActive { get; set; } = true;

    public decimal OpeningBalance { get; set; }

    public int DisplayOrder { get; set; }
}

public class JournalEntry : BaseEntity
{
    [MaxLength(50)]
    public string JournalNo { get; set; } = string.Empty;

    public DateTime EntryDate { get; set; }

    public JournalEntryType EntryType { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    public int? FinancialPeriodId { get; set; }

    public int? ReferenceId { get; set; }

    [MaxLength(100)]
    public string? ReferenceType { get; set; }

    public bool IsPosted { get; set; }

    public PostingStatus PostingStatus { get; set; } = PostingStatus.Pending;

    public DateTime? PostedAt { get; set; }

    [MaxLength(64)]
    public string? PostedBy { get; set; }

    [MaxLength(500)]
    public string? PostingError { get; set; }
}

public class JournalEntryLine : BaseEntity
{
    public int JournalEntryId { get; set; }

    public int AccountId { get; set; }

    public JournalLineType LineType { get; set; }

    public decimal Amount { get; set; }

    [MaxLength(500)]
    public string? Narration { get; set; }
}

public class GeneralLedgerEntry : BaseEntity
{
    public int AccountId { get; set; }

    public DateTime EntryDate { get; set; }

    public int? JournalEntryId { get; set; }

    [MaxLength(50)]
    public string? JournalNo { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public decimal DebitAmount { get; set; }

    public decimal CreditAmount { get; set; }

    public decimal RunningBalance { get; set; }

    public int? FinancialPeriodId { get; set; }
}

public class BankTransaction : BaseEntity
{
    public int AccountId { get; set; }

    public BankAccountType BankAccountType { get; set; }

    public DateTime TransactionDate { get; set; }

    public BankTransactionType TransactionType { get; set; }

    public decimal Amount { get; set; }

    [MaxLength(100)]
    public string? ReferenceNo { get; set; }

    [MaxLength(100)]
    public string? ChequeNo { get; set; }

    public DateTime? ChequeDate { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(100)]
    public string? CounterParty { get; set; }

    public bool IsReconciled { get; set; }

    public DateTime? ReconciledAt { get; set; }

    [MaxLength(64)]
    public string? ReconciledBy { get; set; }

    public int? FinancialPeriodId { get; set; }
}

public class FinancialPeriod : BaseEntity
{
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public FinancialPeriodStatus Status { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime? ClosedAt { get; set; }

    [MaxLength(64)]
    public string? ClosedBy { get; set; }
}

public class FinanceSetting : BaseEntity
{
    [MaxLength(100)]
    public string Key { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string Value { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(50)]
    public string Category { get; set; } = "General";
}

public class AccountMapping : BaseEntity
{
    [MaxLength(100)]
    public string TransactionType { get; set; } = string.Empty;

    [MaxLength(20)]
    public string DebitAccountCode { get; set; } = string.Empty;

    [MaxLength(20)]
    public string CreditAccountCode { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;
}
