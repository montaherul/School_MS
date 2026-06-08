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
    public int? ExamId { get; set; }
    public string? ExamName { get; set; }
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
    public int? ExamId { get; set; }
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
