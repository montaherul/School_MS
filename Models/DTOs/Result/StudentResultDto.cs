namespace SchoolManagementSystem.Models.DTOs.Result;

public class StudentResultExamDto
{
    public int Id { get; set; }
    public int ExamId { get; set; }
    public string ExamName { get; set; } = string.Empty;
    public string Term { get; set; } = string.Empty;
    public DateOnly StartsOn { get; set; }
    public DateOnly EndsOn { get; set; }
    public decimal TotalMarks { get; set; }
    public decimal TotalFullMarks { get; set; }
    public decimal Gpa { get; set; }
    public string Grade { get; set; } = string.Empty;
    public int Position { get; set; }
    public int ClassPosition { get; set; }
    public int? GroupPosition { get; set; }
    public bool IsPassed { get; set; }
    public int FailedSubjectCount { get; set; }
    public int PassedSubjectCount { get; set; }
    public DateTime? PublishedAt { get; set; }
    public int Status { get; set; }
}

public class StudentResultSubjectDto
{
    public int Id { get; set; }
    public int ExamId { get; set; }
    public string ExamName { get; set; } = string.Empty;
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public string SubjectCode { get; set; } = string.Empty;
    public bool IsOptionalSubject { get; set; }
    public bool IsReligionSubject { get; set; }
    public decimal MarksObtained { get; set; }
    public decimal FullMarks { get; set; }
    public decimal PassMarks { get; set; }
    public string Grade { get; set; } = string.Empty;
    public decimal GradePoint { get; set; }
    public bool IsPassed { get; set; }
}

/// <summary>
/// Phase 5: Comprehensive final result DTO for student/academic year summary
/// </summary>
public class StudentFinalResultDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int AcademicYearId { get; set; }
    public string AcademicYearName { get; set; } = string.Empty;

    public decimal FinalGpa { get; set; }
    public decimal WeightedTotalMarks { get; set; }
    public string FinalGrade { get; set; } = string.Empty;

    // All 4 position types
    public int FinalPosition { get; set; }
    public int FinalClassPosition { get; set; }
    public int FinalSectionPosition { get; set; }
    public int FinalGroupPosition { get; set; }

    public int TotalPassedSubjects { get; set; }
    public int TotalFailedSubjects { get; set; }
    public decimal AttendancePercentage { get; set; }
    public bool IsPassed { get; set; }
    public int PromotionStatus { get; set; }
    public string? PromotionRemarks { get; set; }
}