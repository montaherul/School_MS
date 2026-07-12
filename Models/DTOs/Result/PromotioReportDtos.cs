namespace SchoolManagementSystem.Models.DTOs.Result;

public class PromotioRegisterDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentNo { get; set; } = string.Empty;
    public string FromClassName { get; set; } = string.Empty;
    public string ToClassName { get; set; } = string.Empty;
    public string FromSectionName { get; set; } = string.Empty;
    public string ToSectionName { get; set; } = string.Empty;
    public string FromGroupName { get; set; } = string.Empty;
    public string ToGroupName { get; set; } = string.Empty;
    public int? NewRollNumber { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime PromotedAt { get; set; }
    public string? Remarks { get; set; }
}

public class FailedStudentDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentNo { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string SectionName { get; set; } = string.Empty;
    public decimal GPA { get; set; }
    public int TotalFailedSubjects { get; set; }
    public decimal AttendancePercentage { get; set; }
}

public class GraduatedStudentDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentNo { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public decimal FinalGPA { get; set; }
    public string FinalGrade { get; set; } = string.Empty;
    public DateTime GraduatedAt { get; set; }
}

public class ClassPromotioReportDto
{
    public string ClassName { get; set; } = string.Empty;
    public int TotalStudents { get; set; }
    public int PromotedCount { get; set; }
    public int FailedCount { get; set; }
    public int RepeatCount { get; set; }
    public decimal PromotionRate { get; set; }
}

public class SectionPromotioReportDto
{
    public string SectionName { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public int TotalStudents { get; set; }
    public int PromotedCount { get; set; }
    public int FailedCount { get; set; }
    public decimal PromotionRate { get; set; }
}

public class GroupDistributionReportDto
{
    public string GroupName { get; set; } = string.Empty;
    public int TotalStudents { get; set; }
    public int PromotedCount { get; set; }
    public int FailedCount { get; set; }
    public decimal AverageGPA { get; set; }
}
