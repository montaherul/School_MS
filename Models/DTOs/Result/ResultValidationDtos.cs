namespace SchoolManagementSystem.Models.DTOs.Result;

public class ResultValidationRequest
{
    public int ExamId { get; set; }
    public List<int>? ClassIds { get; set; }
    public bool CheckMissingMarks { get; set; } = true;
    public bool CheckDuplicateMarks { get; set; } = true;
    public bool CheckGpaMismatch { get; set; } = true;
    public bool CheckIncompleteComponents { get; set; } = true;
    public bool CheckMissingSubjects { get; set; } = true;
}

public class ResultValidationResultDto
{
    public bool IsValid { get; set; }
    public string ExamName { get; set; } = "";
    public int TotalStudents { get; set; }
    public int TotalSubjects { get; set; }
    public int TotalIssues { get; set; }
    public int PassedCount { get; set; }
    public int FailedCount { get; set; }
    public int IncompleteCount { get; set; }
    public List<ResultValidationIssueDto> Issues { get; set; } = [];
    public List<ResultValidationSummaryItemDto> Summary { get; set; } = [];
}

public class ResultValidationIssueDto
{
    public string Severity { get; set; } = "";
    public string Category { get; set; } = "";
    public int? StudentId { get; set; }
    public string StudentName { get; set; } = "";
    public string SubjectName { get; set; } = "";
    public string Message { get; set; } = "";
    public string? Details { get; set; }
}

public class ResultValidationSummaryItemDto
{
    public string ClassName { get; set; } = "";
    public int TotalStudents { get; set; }
    public int CompletedStudents { get; set; }
    public int IncompleteStudents { get; set; }
}
