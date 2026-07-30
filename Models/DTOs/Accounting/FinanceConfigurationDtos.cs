using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models.DTOs.Accounting;

public class FinanceSettingDto
{
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = "General";
}

public class FinanceSettingUpsertDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Key { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Value { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [StringLength(50)]
    public string Category { get; set; } = "General";
}

public class AccountMappingDto
{
    public int Id { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public string DebitAccountCode { get; set; } = string.Empty;
    public string DebitAccountName { get; set; } = string.Empty;
    public string CreditAccountCode { get; set; } = string.Empty;
    public string CreditAccountName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}

public class AccountMappingUpsertDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string TransactionType { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string DebitAccountCode { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string CreditAccountCode { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;
}

public class PostingRuleDto
{
    public int Id { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public string? DebitAccountCode { get; set; }
    public string? CreditAccountCode { get; set; }
    public bool AutoPost { get; set; }
    public string? Condition { get; set; }
}

public class FiscalSettingDto
{
    public string FiscalYearStart { get; set; } = "01-01";
    public string FiscalYearEnd { get; set; } = "12-31";
    public bool AutoCreatePeriods { get; set; } = true;
    public int GracePeriodDays { get; set; } = 30;
    public decimal WriteOffThreshold { get; set; } = 1.00m;
    public int DefaultDueDay { get; set; } = 10;
    public decimal MinPaymentPercentage { get; set; } = 0;
    public bool EnforcePeriodClosing { get; set; } = true;
}
