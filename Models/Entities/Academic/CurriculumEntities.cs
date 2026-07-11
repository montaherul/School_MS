using SchoolManagementSystem.Models.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models.Entities.Academic;

public class CurriculumVersion : BaseEntity
{
    [MaxLength(200)]
    public string VersionName { get; set; } = string.Empty;

    public int AcademicYearId { get; set; }
    public AcademicYear? AcademicYear { get; set; }

    public DateOnly EffectiveFrom { get; set; }
    public bool IsCurrent { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }
}

public class CurriculumSubject : BaseEntity
{
    public int CurriculumVersionId { get; set; }
    public CurriculumVersion? CurriculumVersion { get; set; }

    public int SubjectId { get; set; }
    public Subject? Subject { get; set; }

    [MaxLength(30)]
    public string SubjectCode { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Category { get; set; } = string.Empty; // Core, Elective, Vocational, Religion

    public int TotalHours { get; set; }
    public bool IsCompulsory { get; set; }
    public int SortOrder { get; set; }
}
