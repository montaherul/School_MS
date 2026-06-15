namespace SchoolManagementSystem.Models.DTOs.Result;

public class MarksEntryStudentDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentNo { get; set; } = string.Empty;
    public string RollNumber { get; set; } = string.Empty;
    public int ClassId { get; set; }
    public int SectionId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string SectionName { get; set; } = string.Empty;
    public int? MarkId { get; set; }
    public decimal? MarksObtained { get; set; }
    public ComponentMarksDto ComponentMarks { get; set; } = new();
    public string? ComponentValues { get; set; }
    public string? Grade { get; set; }
    public decimal? GradePoint { get; set; }
    public bool? IsLocked { get; set; }
    public int? MarkStatus { get; set; }
    public bool? IsAbsent { get; set; }
    public bool HasEntry { get; set; }
}