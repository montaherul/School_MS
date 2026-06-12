using SchoolManagementSystem.Models.ViewModels.Exam;

namespace SchoolManagementSystem.Models.DTOs.Exam;

public class ExamSubjectConfigDto
{
    public int? Id { get; set; }
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public int? TeacherId { get; set; }
    public int TotalWrittenMarks { get; set; }
    public int TotalMCQMarks { get; set; }
    public int TotalPracticalMarks { get; set; }
    public int TotalVivaMarks { get; set; }
    public int TotalAssignmentMarks { get; set; }
    public decimal PassMark { get; set; } = 33;
    public DateOnly? ExamDate { get; set; }
    public TimeOnly? ExamStartTime { get; set; }
    public int? ExamDuration { get; set; }
    public string? RoomNumber { get; set; }
    public bool IsOptional { get; set; }
    public bool IsActive { get; set; } = true;
    public List<TeacherOption>? Teachers { get; set; }
}
