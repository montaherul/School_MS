namespace SchoolManagementSystem.Models.DTOs.Result;

public class MarkEntrySheetDto
{
    public int StudentId { get; set; }
    public string StudentNo { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public int RollNumber { get; set; }
    public decimal? MarksObtained { get; set; }
    public string? Grade { get; set; }
    public bool IsLocked { get; set; }
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
    public string? ComponentValues { get; set; }
    public int? EnteredByTeacherId { get; set; }
    public string? EnteredByTeacherName { get; set; }
}
