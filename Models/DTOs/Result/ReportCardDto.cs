namespace SchoolManagementSystem.Models.DTOs.Result;

public class ReportCardDto
{
    public string SchoolName { get; set; } = string.Empty;
    public string SchoolAddress { get; set; } = string.Empty;
    public string EIIN { get; set; } = string.Empty;
    public string SchoolLogoPath { get; set; } = string.Empty;
    
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
    public string PhotoPath { get; set; } = string.Empty;
    
    public int ExamId { get; set; }
    public string ExamName { get; set; } = string.Empty;
    public string Term { get; set; } = string.Empty;
    public DateOnly StartsOn { get; set; }
    public DateOnly EndsOn { get; set; }
    public int AcademicYearId { get; set; }
    public string AcademicYearName { get; set; } = string.Empty;
    
    public List<ReportCardSubjectDto> Subjects { get; set; } = [];
    public ReportCardSummaryDto Summary { get; set; } = new();
}

public class ReportCardSubjectDto
{
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public string SubjectCode { get; set; } = string.Empty;
    public decimal FullMarks { get; set; }
    public decimal PassMarks { get; set; }
    public decimal MarksObtained { get; set; }
    public string Grade { get; set; } = string.Empty;
    public decimal GradePoint { get; set; }
    public bool IsPassed { get; set; }
    public bool IsOptionalSubject { get; set; }
    public bool IsReligionSubject { get; set; }
    public ComponentMarksDto ComponentMarks { get; set; } = new();
}

public class ReportCardSummaryDto
{
    public decimal TotalMarks { get; set; }
    public decimal TotalFullMarks { get; set; }
    public decimal Gpa { get; set; }
    public string Grade { get; set; } = string.Empty;
    public int Position { get; set; }
    public int ClassPosition { get; set; }
    public int? GroupPosition { get; set; }
    public int SectionPosition { get; set; }
    public bool IsPassed { get; set; }
    public int FailedSubjectCount { get; set; }
    public int PassedSubjectCount { get; set; }
    public int Status { get; set; }
    public DateTime? PublishedAt { get; set; }
}