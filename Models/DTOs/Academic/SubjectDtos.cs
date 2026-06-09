using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models.DTOs.Academic;

public class SubjectListItemDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string NameBn { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string? SubjectGroup { get; set; }
    public bool IsReligionSubject { get; set; }
    public string? ReligionType { get; set; }
    public bool IsOptional { get; set; }
    public bool IsPractical { get; set; }
    public decimal DefaultFullMarks { get; set; }
    public decimal DefaultPassMarks { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}

public class SubjectUpsertDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(30)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string NameBn { get; set; } = string.Empty;

    [StringLength(30)]
    public string ShortName { get; set; } = string.Empty;

    [StringLength(50)]
    public string Category { get; set; } = string.Empty;

    public string? SubjectGroup { get; set; }
    public bool IsReligionSubject { get; set; }
    public string? ReligionType { get; set; }
    public bool IsOptional { get; set; }
    public bool IsPractical { get; set; }
    public decimal DefaultFullMarks { get; set; } = 100;
    public decimal DefaultPassMarks { get; set; } = 33;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}
