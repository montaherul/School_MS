using SchoolManagementSystem.Services.Interfaces.Result;

namespace SchoolManagementSystem.Services.Interfaces.Result;

/// <summary>
/// Service for managing mark entry workflow
/// Handles draft/submit/lock workflow with teacher scope validation
/// </summary>
public interface IMarkEntryService
{
    /// <summary>
    /// Gets mark entry data for a teacher to enter marks
    /// Validates teacher has permission for the subject/class
    /// </summary>
    Task<MarkEntrySheet> GetMarkEntrySheetAsync(int examId, int subjectId, int teacherId);

    /// <summary>
    /// Saves marks as draft (not finalized)
    /// </summary>
    Task SaveDraftMarksAsync(MarkEntrySheet sheet, int teacherId);

    /// <summary>
    /// Submits marks for approval (finalizes entry)
    /// </summary>
    Task SubmitMarksAsync(int examId, int subjectId, int teacherId);

    /// <summary>
    /// Validates teacher has permission to enter marks for subject/class
    /// </summary>
    Task<bool> ValidateTeacherPermissionAsync(int examId, int subjectId, int teacherId);

    /// <summary>
    /// Locks marks to prevent further editing
    /// </summary>
    Task LockMarksAsync(int examId, int subjectId, int adminId);

    /// <summary>
    /// Unlocks marks for correction (admin only)
    /// </summary>
    Task UnlockMarksAsync(int examId, int subjectId, int adminId, string reason);

    /// <summary>
    /// Bulk import marks from Excel/CSV
    /// </summary>
    Task<BulkImportResult> BulkImportMarksAsync(Stream fileStream, int examId, int subjectId, int teacherId);

    /// <summary>
    /// Gets mark entry audit trail
    /// </summary>
    Task<IEnumerable<MarkAuditEntry>> GetMarkAuditTrailAsync(int examId, int subjectId);

    /// <summary>
    /// Recalculates marks after component changes
    /// </summary>
    Task RecalculateMarksAsync(int examId, int subjectId);
}

public class MarkEntrySheet
{
    public int ExamId { get; set; }
    public int SubjectId { get; set; }
    public string ExamName { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public List<StudentMarkEntry> StudentMarks { get; set; } = [];
    public MarkEntryConfig Config { get; set; } = new();
    public bool IsLocked { get; set; }
    public bool IsSubmitted { get; set; }
}

public class StudentMarkEntry
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int RollNumber { get; set; }
    public decimal? WrittenMarks { get; set; }
    public decimal? MCQMarks { get; set; }
    public decimal? CQMarks { get; set; }
    public decimal? PracticalMarks { get; set; }
    public decimal? VivaMarks { get; set; }
    public decimal? LabMarks { get; set; }
    public decimal? OralMarks { get; set; }
    public decimal? AssignmentMarks { get; set; }
    public decimal? ContinuousAssessmentMarks { get; set; }
    public decimal? CompetencyMarks { get; set; }
    public decimal? BehaviourMarks { get; set; }
    public decimal? ParticipationMarks { get; set; }
    public decimal TotalMarks { get; set; }
    public bool IsPresent { get; set; } = true;
}

public class MarkEntryConfig
{
    public decimal FullMarks { get; set; } = 100;
    public decimal PassMarks { get; set; } = 33;
    public bool HasWritten { get; set; } = true;
    public bool HasMCQ { get; set; }
    public bool HasCQ { get; set; }
    public bool HasPractical { get; set; }
    public bool HasViva { get; set; }
    public bool HasLab { get; set; }
    public bool HasOral { get; set; }
    public bool HasAssignment { get; set; }
    public bool HasContinuousAssessment { get; set; }
    public bool HasCompetency { get; set; }
    public bool HasBehaviour { get; set; }
    public bool HasParticipation { get; set; }
}

public class BulkImportResult
{
    public int SuccessCount { get; set; }
    public int ErrorCount { get; set; }
    public List<string> Errors { get; set; } = [];
    public List<string> Warnings { get; set; } = [];
}

public class MarkAuditEntry
{
    public DateTime ChangedAt { get; set; }
    public string ChangedBy { get; set; } = string.Empty;
    public string ChangeType { get; set; } = string.Empty;
    public decimal OldMarks { get; set; }
    public decimal NewMarks { get; set; }
    public string Reason { get; set; } = string.Empty;
}