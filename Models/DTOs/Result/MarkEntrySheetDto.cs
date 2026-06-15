namespace SchoolManagementSystem.Models.DTOs.Result;

public class MarkEntrySheetDto
{
    public int StudentId { get; set; }
    public string StudentNo { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public int RollNumber { get; set; }
    public decimal? MarksObtained { get; set; }
    public string? Grade { get; set; }
    public bool IsLocked { get; set; }
    public ComponentMarksDto ComponentMarks { get; set; } = new();
    public string? ComponentValues { get; set; }
    public int? EnteredByTeacherId { get; set; }
    public string? EnteredByTeacherName { get; set; }
}
