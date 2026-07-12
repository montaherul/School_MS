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
    public string SubjectGroup { get; set; } = string.Empty;
    public bool IsReligionSubject { get; set; }
    public string? ReligionType { get; set; }
    public bool IsOptional { get; set; }
    public bool IsPractical { get; set; }
    public decimal DefaultFullMarks { get; set; }
    public decimal DefaultPassMarks { get; set; }
    public decimal TheoryMarks { get; set; }
    public decimal PracticalMarks { get; set; }
    public decimal PassMarks { get; set; }
    public decimal Credit { get; set; }
    public string? NctbCode { get; set; }
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

    [Required(ErrorMessage = "Subject Group is required. Select General for classes 1-8 or the appropriate stream for classes 9-10.")]
    [RegularExpression("^(General|Science|BusinessStudies|Humanities|Religion|Optional)$", ErrorMessage = "Subject Group must be one of: General, Science, BusinessStudies, Humanities, Religion, Optional.")]
    public string SubjectGroup { get; set; } = string.Empty;
    public bool IsReligionSubject { get; set; }
    public string? ReligionType { get; set; }
    public bool IsOptional { get; set; }
    public bool IsPractical { get; set; }
    public decimal DefaultFullMarks { get; set; } = 100;
    public decimal DefaultPassMarks { get; set; } = 33;
    public decimal TheoryMarks { get; set; } = 100;
    public decimal PracticalMarks { get; set; } = 0;
    public decimal PassMarks { get; set; } = 33;
    public decimal Credit { get; set; } = 1;

    [StringLength(30)]
    public string? NctbCode { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}
