using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Models.DTOs.Accounting;

public class BankBookEntryDto
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public string AccountName { get; set; } = string.Empty;
    public string BankAccountType { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? ReferenceNo { get; set; }
    public string? ChequeNo { get; set; }
    public string? Description { get; set; }
    public string? CounterParty { get; set; }
    public bool IsReconciled { get; set; }
    public decimal RunningBalance { get; set; }
    public int TotalRecords { get; set; }
}

public class BankBookSummaryDto
{
    public decimal OpeningBalance { get; set; }
    public decimal TotalDeposits { get; set; }
    public decimal TotalWithdrawals { get; set; }
    public decimal ClosingBalance { get; set; }
    public decimal UnclearedBalance { get; set; }
}

public class BankTransactionDto
{
    public int AccountId { get; set; }
    public int BankAccountType { get; set; }
    public DateTime TransactionDate { get; set; } = DateTime.Today;
    public int TransactionType { get; set; }
    public decimal Amount { get; set; }
    public string? ReferenceNo { get; set; }
    public string? ChequeNo { get; set; }
    public string? Description { get; set; }
    public string? CounterParty { get; set; }
    public int? FinancialPeriodId { get; set; }
}

public class BankReconciliationDto
{
    public int TransactionId { get; set; }
    public DateTime TransactionDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ReferenceNo { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public bool IsReconciled { get; set; }
}
