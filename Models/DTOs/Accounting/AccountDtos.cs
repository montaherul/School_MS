using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Models.DTOs.Accounting;

public class AccountListItemDto
{
    public int Id { get; set; }
    public string AccountCode { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string AccountType { get; set; } = string.Empty;
    public string? ParentAccount { get; set; }
    public bool IsActive { get; set; }
    public decimal OpeningBalance { get; set; }
    public int DisplayOrder { get; set; }
    public int TotalRecords { get; set; }
}

public class AccountUpsertDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(20)]
    public string AccountCode { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string AccountName { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    public AccountType AccountType { get; set; }

    public int? ParentAccountId { get; set; }

    public bool IsActive { get; set; } = true;

    public decimal OpeningBalance { get; set; }

    public int DisplayOrder { get; set; }
}

public class AccountTreeDto
{
    public int Id { get; set; }
    public string AccountCode { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public List<AccountTreeDto> Children { get; set; } = [];
}
