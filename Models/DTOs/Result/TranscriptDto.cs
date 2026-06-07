namespace SchoolManagementSystem.Models.DTOs.Result;

public class TranscriptStudentInfoDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentNo { get; set; } = string.Empty;
    public string RollNumber { get; set; } = string.Empty;
    public DateOnly? DateOfBirth { get; set; }
    public string FatherName { get; set; } = string.Empty;
    public string MotherName { get; set; } = string.Empty;
    public int ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public int? SectionId { get; set; }
    public string SectionName { get; set; } = string.Empty;
    public int? StudentGroupId { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public string CurrentAcademicYear { get; set; } = string.Empty;
}

public class TranscriptExamResultDto
{
    public int AcademicYearId { get; set; }
    public string AcademicYearName { get; set; } = string.Empty;
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
    public bool IsPassed { get; set; }
    public int FailedSubjectCount { get; set; }
    public int PassedSubjectCount { get; set; }
}

public class TranscriptSubjectResultDto
{
    public int ExamId { get; set; }
    public string ExamName { get; set; } = string.Empty;
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public string SubjectCode { get; set; } = string.Empty;
    public decimal MarksObtained { get; set; }
    public decimal FullMarks { get; set; }
    public decimal PassMarks { get; set; }
    public string Grade { get; set; } = string.Empty;
    public decimal GradePoint { get; set; }
    public bool IsPassed { get; set; }
}

public class TranscriptOverallStatsDto
{
    public int TotalExamsTaken { get; set; }
    public int TotalAcademicYears { get; set; }
    public decimal AverageGPA { get; set; }
    public decimal BestGPA { get; set; }
    public int PassedExams { get; set; }
    public int FailedExams { get; set; }
}