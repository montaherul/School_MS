using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models.DTOs.Academic;

public class LessonPlanListItemDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public string TeacherName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime LessonDate { get; set; }
    public bool IsActive { get; set; }
    public int TotalRecords { get; set; }
}

public class LessonPlanUpsertDto
{
    public int Id { get; set; }

    [Required, StringLength(200)]
    public string Title { get; set; } = string.Empty;

    public string? Objectives { get; set; }
    public string? Materials { get; set; }
    public string? Procedure { get; set; }

    [StringLength(500)]
    public string? AssessmentMethod { get; set; }

    public int? DurationMinutes { get; set; }
    public DateTime LessonDate { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    [Required]
    public int TeacherId { get; set; }

    [Required]
    public int SchoolClassId { get; set; }

    [Required]
    public int SubjectId { get; set; }

    [Required]
    public int AcademicYearId { get; set; }

    public string Status { get; set; } = "Draft";
    public bool IsActive { get; set; } = true;
}
