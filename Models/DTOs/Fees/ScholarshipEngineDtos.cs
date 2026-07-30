namespace SchoolManagementSystem.Models.DTOs.Fees;

public class ScholarshipEngineResultDto
{
    public int StudentsProcessed { get; set; }
    public int ScholarshipsApplied { get; set; }
    public decimal TotalDiscountAmount { get; set; }
    public List<string> Errors { get; set; } = [];
}

public class AllocationEngineResultDto
{
    public int PaymentsProcessed { get; set; }
    public int AllocationsCreated { get; set; }
    public decimal TotalAllocated { get; set; }
    public List<string> Errors { get; set; } = [];
}

public class AutoWriteOffResultDto
{
    public int InvoicesWrittenOff { get; set; }
    public decimal TotalWrittenOff { get; set; }
    public List<string> Errors { get; set; } = [];
}

public class BadDebtResultDto
{
    public int InvoicesMarked { get; set; }
    public decimal TotalAmount { get; set; }
    public int JournalEntryId { get; set; }
    public List<string> Errors { get; set; } = [];
}

public class WriteOffConfigDto
{
    public decimal Threshold { get; set; } = 1.00m;
    public string AccountCode { get; set; } = "4-301";
    public string AccountName { get; set; } = "Bad Debt Expense";
    public int AccountType { get; set; } = 4;
}
