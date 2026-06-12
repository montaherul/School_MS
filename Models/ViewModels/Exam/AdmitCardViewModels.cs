namespace SchoolManagementSystem.Models.ViewModels.Exam;

public class AdmitCardViewModel
{
    public int ExamId { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string? StudentPhotoPath { get; set; }
    public int? RollNumber { get; set; }
    public string? AdmitCardNumber { get; set; }
    public string? SeatNumber { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string SectionName { get; set; } = string.Empty;
    public string? GroupName { get; set; }
    public string AcademicYear { get; set; } = string.Empty;
    public string ExamName { get; set; } = string.Empty;
    public string ExamType { get; set; } = string.Empty;
    public DateOnly ExamStartDate { get; set; }
    public DateOnly ExamEndDate { get; set; }
    public string SchoolName { get; set; } = string.Empty;
    public string SchoolAddress { get; set; } = string.Empty;
    public string? SchoolLogo { get; set; }
    public string EIIN { get; set; } = string.Empty;
    public bool IsIssued { get; set; }
    public DateTime? IssuedAt { get; set; }
    public List<AdmitCardSubjectRow> SubjectSchedules { get; set; } = [];
}

public class AdmitCardSubjectRow
{
    public string SubjectName { get; set; } = string.Empty;
    public DateOnly ExamDate { get; set; }
    public string StartTime { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public string RoomNumber { get; set; } = string.Empty;
}
