using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models.DTOs.Academic;

public class ClassSubjectListItemDto
{
    public int Id { get; set; }
    public int SchoolClassId { get; set; }
    public string SchoolClassName { get; set; } = string.Empty;

    public int SubjectId { get; set; }
    public string SubjectCode { get; set; } = string.Empty;
    public string SubjectNameEn { get; set; } = string.Empty;
    public string SubjectNameBn { get; set; } = string.Empty;

    public int? StudentGroupId { get; set; }
    public string? GroupName { get; set; }

    public decimal FullMarks { get; set; }
    public decimal PassMarks { get; set; }

    public bool IsMandatory { get; set; }
    public bool IsOptional { get; set; }
    public bool IsReligionSubject { get; set; }
    public string? ReligionType { get; set; }

    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}

public class ClassSubjectUpsertDto
{
    public int Id { get; set; }

    [Required]
    public int SchoolClassId { get; set; }

    [Required]
    public int SubjectId { get; set; }

    public int? StudentGroupId { get; set; }
    public string? GroupName { get; set; }

    [Required]
    [Range(0, 500)]
    public decimal FullMarks { get; set; } = 100;

    [Required]
    [Range(0, 500)]
    public decimal PassMarks { get; set; } = 33;

    public int DisplayOrder { get; set; }

    public bool IsMandatory { get; set; } = true;
    public bool IsOptional { get; set; } = false;
    public bool IsReligionSubject { get; set; } = false;
    public string? ReligionType { get; set; }

    public bool IsActive { get; set; } = true;
}

public class ClassSubjectAssignmentDto
{
    [Required]
    public int SchoolClassId { get; set; }

    public string? GroupName { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "Please select at least one subject to map.")]
    public List<int> SubjectIds { get; set; } = [];

    public decimal FullMarks { get; set; } = 100;
    public decimal PassMarks { get; set; } = 33;
}
