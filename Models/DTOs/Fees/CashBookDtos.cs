namespace SchoolManagementSystem.Models.DTOs.Fees;

public class CashBookDayDto
{
    public decimal OpeningBalance { get; set; }
    public DateTime TransactionDate { get; set; }
    public decimal CashIn { get; set; }
    public decimal CashOut { get; set; }
    public decimal NetChange { get; set; }
    public decimal ClosingBalance { get; set; }
    public int PaymentCount { get; set; }
    public int RefundCount { get; set; }
    public int EntryCount { get; set; }
}

public class CashBookResultDto
{
    public decimal OpeningBalance { get; set; }
    public decimal TotalCashIn { get; set; }
    public decimal TotalCashOut { get; set; }
    public decimal ClosingBalance { get; set; }
    public List<CashBookDayDto> Days { get; set; } = [];
}
