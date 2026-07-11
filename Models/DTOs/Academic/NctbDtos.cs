using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models.DTOs.Academic;

public class NctbComplianceReportDto
{
    public int AcademicYearId { get; set; }
    public string AcademicYearName { get; set; } = string.Empty;
    public double ComplianceScore { get; set; }
    public int TotalChecks { get; set; }
    public int PassedChecks { get; set; }

    public bool HasScienceGroup { get; set; }
    public bool HasBusinessStudiesGroup { get; set; }
    public bool HasHumanitiesGroup { get; set; }
    public bool HasCompulsoryCoreSubjects { get; set; }
    public bool HasAllReligionTypes { get; set; }

    public int VocationalSubjectCount { get; set; }
    public int TotalSubjectCount { get; set; }
    public int GroupCount { get; set; }
    public int ReligionSubjectCount { get; set; }

    public List<string> MissingSubjects { get; set; } = [];
    public List<string> Warnings { get; set; } = [];
    public List<string> Recommendations { get; set; } = [];

    public List<NctbChecklistItem> Checklist { get; set; } = [];
    public List<SubjectCategoryBreakdown> SubjectCategoryBreakdown { get; set; } = [];
}

public class NctbChecklistItem
{
    public string Id { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool Passed { get; set; }
    public string Severity { get; set; } = "info";
}

public class SubjectCategoryBreakdown
{
    public string Category { get; set; } = string.Empty;
    public int Count { get; set; }
    public List<string> Subjects { get; set; } = [];
}

public class CurriculumVersionUpsertDto
{
    [Required(ErrorMessage = "Version name is required")]
    [MaxLength(200)]
    public string VersionName { get; set; } = string.Empty;

    [Required]
    public int AcademicYearId { get; set; }

    [Required]
    public DateOnly EffectiveFrom { get; set; }

    public bool IsCurrent { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }
}

public class CurriculumVersionDto
{
    public int Id { get; set; }
    public string VersionName { get; set; } = string.Empty;
    public int AcademicYearId { get; set; }
    public string AcademicYearName { get; set; } = string.Empty;
    public DateOnly EffectiveFrom { get; set; }
    public bool IsCurrent { get; set; }
    public string? Description { get; set; }
}

public class CurriculumSubjectDto
{
    public int Id { get; set; }
    public int CurriculumVersionId { get; set; }
    public int SubjectId { get; set; }
    public string SubjectCode { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int TotalHours { get; set; }
    public bool IsCompulsory { get; set; }
    public int SortOrder { get; set; }
}
