using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Entities.Base;
using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Models.Entities.Result;

public class MarkEntry : BaseEntity
{
    public int ExamId { get; set; }
    public int StudentId { get; set; }
    public int SubjectId { get; set; }
    public decimal MarksObtained { get; set; }
    public int EnteredByTeacherId { get; set; }
    public PublishStatus Status { get; set; } = PublishStatus.Draft;
}

public class GradingRule : BaseEntity
{
    [MaxLength(10)]
    public string Grade { get; set; } = string.Empty;

    public decimal MinMarks { get; set; }
    public decimal MaxMarks { get; set; }
    public decimal GradePoint { get; set; }
}

public class ResultPublication : BaseEntity
{
    public int ExamId { get; set; }
    public PublishStatus Status { get; set; } = PublishStatus.PendingApproval;
    public DateTime? PublishedAt { get; set; }
    public int? ApprovedByUserId { get; set; }
}

public class ReportCard : BaseEntity
{
    public int ExamId { get; set; }
    public int StudentId { get; set; }

    [MaxLength(260)]
    public string PdfPath { get; set; } = string.Empty;

    public decimal Gpa { get; set; }
}
