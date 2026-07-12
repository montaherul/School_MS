using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models.DTOs.Exam;

/// <summary>
/// Read model for ExamComponent list/grid displays.
/// </summary>
public class ExamComponentListDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
    public decimal DefaultFullMarks { get; set; }
    public decimal DefaultPassMarks { get; set; }
    public bool IsPractical { get; set; }
    public bool IsOptional { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Create/update model for ExamComponent.
/// </summary>
public class ExamComponentUpsertDto
{
    public int? Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public int DisplayOrder { get; set; } = 0;
    public decimal DefaultFullMarks { get; set; } = 100;
    public decimal DefaultPassMarks { get; set; } = 33;
    public bool IsPractical { get; set; } = false;
    public bool IsOptional { get; set; } = false;
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Read model for SubjectMarkStructure (component-level mark config per subject/exam).
/// </summary>
public class SubjectMarkStructureDto
{
    public int Id { get; set; }
    public int ComponentId { get; set; }
    public string ComponentName { get; set; } = string.Empty;
    public string ComponentCode { get; set; } = string.Empty;
    public int? ClassId { get; set; }
    public string? ClassName { get; set; }
    public int? SubjectId { get; set; }
    public string? SubjectName { get; set; }
    public int? StudentGroupId { get; set; }
    public string? StudentGroupName { get; set; }
    public decimal FullMarks { get; set; }
    public decimal PassMarks { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// Create/update model for SubjectMarkStructure.
/// </summary>
public class SubjectMarkStructureUpsertDto
{
    public int? Id { get; set; }
    public int ComponentId { get; set; }
    public int? ClassId { get; set; }
    public int? SubjectId { get; set; }
    public int? StudentGroupId { get; set; }
    public decimal FullMarks { get; set; } = 100;
    public decimal PassMarks { get; set; } = 33;
    public int DisplayOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Dynamic grid column definition generated from SubjectMarkStructure.
/// Used by the marks entry sheet to render the correct component columns.
/// </summary>
public class ComponentColumnDto
{
    public int ComponentId { get; set; }
    public string ComponentName { get; set; } = string.Empty;
    public string ComponentCode { get; set; } = string.Empty;
    public decimal FullMarks { get; set; }
    public decimal PassMarks { get; set; }
    public int DisplayOrder { get; set; }
    /// <summary>
    /// JSON field name used by Tabulator for this column.
    /// For standard components: camelCase of the mapped MarkEntry property + "Marks" (e.g. "writtenMarks").
    /// For dynamic components: "cmp_" + ComponentCode (e.g. "cmp_PROJECT").
    /// </summary>
    public string FieldName { get; set; } = string.Empty;
}

/// <summary>
/// Grid configuration for a marks entry sheet — replaces hardcoded column lists.
/// </summary>
public class MarksEntryGridConfigDto
{
    public int ExamId { get; set; }
    public int SubjectId { get; set; }
    public int? ClassId { get; set; }
    public int? StudentGroupId { get; set; }
    public List<ComponentColumnDto> Columns { get; set; } = [];
}

/// <summary>
/// Teacher's assigned exam subject summary.
/// </summary>
public class TeacherExamSubjectDto
{
    public int ExamSubjectId { get; set; }
    public int ExamId { get; set; }
    public string ExamName { get; set; } = string.Empty;
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public int ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public int? SectionId { get; set; }
    public string SectionName { get; set; } = string.Empty;
    public int? StudentGroupId { get; set; }
    public string StudentGroupName { get; set; } = string.Empty;
    public decimal FullMarks { get; set; }
    public string ExamStatus { get; set; } = string.Empty;
    public bool IsLocked { get; set; }
    public bool CanCustomize { get; set; }
    public int ComponentCount { get; set; }
}

/// <summary>
/// Read model for teacher-viewable exam subject component.
/// </summary>
public class TeacherExamSubjectComponentDto
{
    public int Id { get; set; }
    public int ExamSubjectId { get; set; }
    public int ComponentId { get; set; }
    public string ComponentName { get; set; } = string.Empty;
    public string ComponentCode { get; set; } = string.Empty;
    public decimal MaxMarks { get; set; }
    public decimal PassMarks { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsCustomized { get; set; }
    public decimal OriginalMaxMarks { get; set; }
    public decimal OriginalPassMarks { get; set; }
}

/// <summary>
/// Create/update model for teacher component customization.
/// </summary>
public class TeacherExamSubjectComponentUpsertDto
{
    public int Id { get; set; }
    public int ExamSubjectId { get; set; }
    public int ComponentId { get; set; }

    [Required]
    [Range(1, 9999, ErrorMessage = "Max marks must be between 1 and 9999.")]
    public decimal MaxMarks { get; set; }

    [Required]
    [Range(0, 9999, ErrorMessage = "Pass marks must be between 0 and 9999.")]
    public decimal PassMarks { get; set; }

    public int DisplayOrder { get; set; }
}

/// <summary>
/// Grid configuration for teacher marks entry — includes dynamic columns from components.
/// </summary>
public class TeacherMarksEntryGridConfigDto
{
    public int ExamId { get; set; }
    public int ExamSubjectId { get; set; }
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public int ClassId { get; set; }
    public int SectionId { get; set; }
    public int? StudentGroupId { get; set; }
    public List<TeacherExamSubjectComponentDto> Components { get; set; } = [];
}