namespace SchoolManagementSystem.Models.DTOs.Result;

public class PromotionWizardStateDto
{
    public int Step { get; set; } = 1;
    public int FromAcademicYearId { get; set; }
    public string FromAcademicYearName { get; set; } = "";
    public int ToAcademicYearId { get; set; }
    public string ToAcademicYearName { get; set; } = "";
    public int? FromClassId { get; set; }
    public int? ToClassId { get; set; }
    public int? ExamId { get; set; }
    public PromotionWizardPreviewDto? Preview { get; set; }
}

public class PromotionWizardPreviewDto
{
    public int EligibleCount { get; set; }
    public int ConditionalCount { get; set; }
    public int FailedCount { get; set; }
    public int TcCount { get; set; }
    public int InactiveCount { get; set; }
    public int TotalStudents { get; set; }
    public List<PromotionWizardStudentDto> Students { get; set; } = [];
    public List<RollPreviewItem> RollPreview { get; set; } = [];
    public List<SubjectPreviewItem> SubjectPreview { get; set; } = [];
    public string RollStrategy { get; set; } = "MeritBased";
}

public class PromotionWizardStudentDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = "";
    public int RollNumber { get; set; }
    public decimal GPA { get; set; }
    public string Grade { get; set; } = "";
    public double AttendancePercent { get; set; }
    public string Status { get; set; } = "";
    public string? Reason { get; set; }
}

public class PromotionWizardExecuteRequest
{
    public int FromAcademicYearId { get; set; }
    public int ToAcademicYearId { get; set; }
    public int FromClassId { get; set; }
    public int ToClassId { get; set; }
    public int? ExamId { get; set; }
    public bool AutoGenerateRoll { get; set; } = true;
    public bool AutoAssignSubjects { get; set; } = true;
    public bool AutoCreateAttendance { get; set; } = true;
}

public class PromotionWizardExecuteResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";
    public int PromotedCount { get; set; }
    public int RepeatCount { get; set; }
    public int FailedCount { get; set; }
    public int RollCount { get; set; }
    public List<string> Warnings { get; set; } = [];
}

public class RollPreviewItem
{
    public int Rank { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = "";
    public decimal Gpa { get; set; }
    public int ProposedRoll { get; set; }
    public string Strategy { get; set; } = "MeritBased";
}

public class SubjectPreviewItem
{
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = "";
    public string SubjectCode { get; set; } = "";
    public string ClassName { get; set; } = "";
    public bool IsMandatory { get; set; }
}
