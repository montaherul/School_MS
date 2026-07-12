namespace SchoolManagementSystem.Models.DTOs.Exam;

public class AutoTeacherAssignmentResultDto
{
    public int ExamId { get; set; }
    public string ExamName { get; set; } = string.Empty;
    public int TotalSubjects { get; set; }
    public int Assigned { get; set; }
    public int Skipped { get; set; }
    public bool Success { get; set; } = true;
    public string Message { get; set; } = string.Empty;
    public List<AutoAssignmentDetailDto> Details { get; set; } = [];
}

public class AutoAssignmentDetailDto
{
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public bool WasAssigned { get; set; }
    public int? TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}
