using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Models.DTOs.Exam;

/// <summary>
/// Read model for exam list/grid displays. Not for create/update operations.
/// </summary>
public class ExamListDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ExamTerm Term { get; set; }
    public DateOnly StartsOn { get; set; }
    public DateOnly EndsOn { get; set; }
    public ResultWorkflowStatus Status { get; set; }
    public int AcademicYearId { get; set; }
    public string AcademicYearName { get; set; } = string.Empty;
    public int ClassId { get; set; }
    public int? StudentGroupId { get; set; }
    public bool IsLocked { get; set; }
    public int SubjectCount { get; set; }
    public int StudentResultCount { get; set; }
    public DateTime CreatedAt { get; set; }

    /// <summary>Group key for UI grouping of same-named exams across classes.</summary>
    public string ExamGroupKey => SchoolManagementSystem.Models.Entities.Exam.Exam.GenerateGroupKey(AcademicYearId, Name);
}
