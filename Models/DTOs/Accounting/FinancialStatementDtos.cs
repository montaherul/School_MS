namespace SchoolManagementSystem.Models.DTOs.Accounting;

public class IncomeStatementDto
{
    public List<FinancialStatementLine> Incomes { get; set; } = [];
    public decimal TotalIncome => Incomes.Sum(i => i.Amount);
    public List<FinancialStatementLine> Expenses { get; set; } = [];
    public decimal TotalExpense => Expenses.Sum(e => e.Amount);
    public decimal NetProfit => TotalIncome - TotalExpense;
    public string? PeriodName { get; set; }
}

public class BalanceSheetDto
{
    public List<FinancialStatementLine> Assets { get; set; } = [];
    public decimal TotalAssets => Assets.Sum(a => a.Amount);
    public List<FinancialStatementLine> Liabilities { get; set; } = [];
    public decimal TotalLiabilities => Liabilities.Sum(l => l.Amount);
    public List<FinancialStatementLine> Equity { get; set; } = [];
    public decimal TotalEquity => Equity.Sum(e => e.Amount);
    public decimal TotalLiabilitiesEquity => TotalLiabilities + TotalEquity;
    public bool IsBalanced => Math.Round(TotalAssets, 2) == Math.Round(TotalLiabilitiesEquity, 2);
    public string? PeriodName { get; set; }
}

public class FinancialStatementLine
{
    public int AccountId { get; set; }
    public string AccountCode { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

public class MonthlyIncomeSummaryDto
{
    public string Month { get; set; } = string.Empty;
    public int Year { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal NetProfit => TotalIncome - TotalExpense;
}
