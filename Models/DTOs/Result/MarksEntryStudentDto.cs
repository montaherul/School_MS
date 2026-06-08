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
    public decimal? WrittenMarks { get; set; }
    public decimal? MCQMarks { get; set; }
    public decimal? CQMarks { get; set; }
    public decimal? PracticalMarks { get; set; }
    public decimal? AssignmentMarks { get; set; }
    public decimal? VivaMarks { get; set; }
    public decimal? LabMarks { get; set; }
    public decimal? ContinuousAssessmentMarks { get; set; }
    public decimal? OralMarks { get; set; }
    public decimal? CompetencyMarks { get; set; }
    public decimal? BehaviourMarks { get; set; }
    public decimal? ParticipationMarks { get; set; }
    public string? ComponentValues { get; set; }
    public string? Grade { get; set; }
    public decimal? GradePoint { get; set; }
    public bool? IsLocked { get; set; }
    public int? MarkStatus { get; set; }
    public bool? IsAbsent { get; set; }
    public bool HasEntry { get; set; }
}