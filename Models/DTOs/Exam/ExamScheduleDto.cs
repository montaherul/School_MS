namespace SchoolManagementSystem.Models.DTOs.Exam;

public class ExamScheduleDto
{
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public DateOnly ExamDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public int? ClassId { get; set; }
    public int? StudentGroupId { get; set; }
    public int? SectionId { get; set; }
}
