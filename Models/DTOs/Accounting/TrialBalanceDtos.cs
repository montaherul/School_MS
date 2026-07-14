namespace SchoolManagementSystem.Models.DTOs.Accounting;

public class TrialBalanceDto
{
    public int AccountId { get; set; }
    public string AccountCode { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;
    public decimal OpeningDebit { get; set; }
    public decimal OpeningCredit { get; set; }
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
    public decimal ClosingDebit { get; set; }
    public decimal ClosingCredit { get; set; }
}

public class TrialBalanceResultDto
{
    public List<TrialBalanceDto> Entries { get; set; } = [];
    public decimal TotalOpeningDebit => Entries.Sum(e => e.OpeningDebit);
    public decimal TotalOpeningCredit => Entries.Sum(e => e.OpeningCredit);
    public decimal TotalDebit => Entries.Sum(e => e.Debit);
    public decimal TotalCredit => Entries.Sum(e => e.Credit);
    public decimal TotalClosingDebit => Entries.Sum(e => e.ClosingDebit);
    public decimal TotalClosingCredit => Entries.Sum(e => e.ClosingCredit);
    public bool IsBalanced =>
        Math.Round(TotalOpeningDebit, 2) == Math.Round(TotalOpeningCredit, 2) &&
        Math.Round(TotalDebit, 2) == Math.Round(TotalCredit, 2) &&
        Math.Round(TotalClosingDebit, 2) == Math.Round(TotalClosingCredit, 2);
}
