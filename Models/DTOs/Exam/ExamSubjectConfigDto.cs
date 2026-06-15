using SchoolManagementSystem.Models.ViewModels.Exam;
using SchoolManagementSystem.Services.Interfaces.Result;

namespace SchoolManagementSystem.Models.DTOs.Exam;

public class ExamSubjectConfigDto
{
    public int? Id { get; set; }
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public int? TeacherId { get; set; }
    public decimal FullMarks { get; set; }
    public decimal PassMark { get; set; } = 33;
    public DateOnly? ExamDate { get; set; }
    public TimeOnly? ExamStartTime { get; set; }
    public int? ExamDuration { get; set; }
    public string? RoomNumber { get; set; }
    public bool IsOptional { get; set; }
    public bool IsActive { get; set; } = true;
    public List<TeacherOption>? Teachers { get; set; }
    public List<ComponentDetailDto>? ComponentPreview { get; set; }
}
