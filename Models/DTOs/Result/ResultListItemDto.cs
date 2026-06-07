namespace SchoolManagementSystem.Models.DTOs.Result;

public class ResultListItemDto
{
    public int Id { get; set; }
    public int ExamId { get; set; }
    public string ExamName { get; set; } = string.Empty;
    public string Term { get; set; } = string.Empty;
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentNo { get; set; } = string.Empty;
    public string RollNumber { get; set; } = string.Empty;
    public int ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public int? SectionId { get; set; }
    public string SectionName { get; set; } = string.Empty;
    public int? StudentGroupId { get; set; }
    public string GroupName { get; set; } = string.Empty;
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
    public int Status { get; set; }
    public DateTime? PublishedAt { get; set; }
}

public class ResultSummaryStatsDto
{
    public int TotalStudents { get; set; }
    public int PassedCount { get; set; }
    public int FailedCount { get; set; }
    public decimal AverageGPA { get; set; }
    public decimal HighestGPA { get; set; }
    public decimal LowestGPA { get; set; }
    public decimal PassPercentage { get; set; }
}

public class GradeDistributionDto
{
    public string Grade { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class ClassWiseResultDto
{
    public int ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public int TotalStudents { get; set; }
    public int PassedCount { get; set; }
    public decimal AverageGPA { get; set; }
}

public class GroupWiseResultDto
{
    public int? StudentGroupId { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public int TotalStudents { get; set; }
    public int PassedCount { get; set; }
    public decimal AverageGPA { get; set; }
}

public class TopStudentDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string RollNumber { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public decimal Gpa { get; set; }
    public string Grade { get; set; } = string.Empty;
    public int Position { get; set; }
    public int ClassPosition { get; set; }
}