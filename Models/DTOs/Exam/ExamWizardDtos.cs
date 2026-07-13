using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models.DTOs.Exam;

public class ExamCreationPreviewDto
{
    public List<ExamClassHierarchyItemDto> ClassHierarchy { get; set; } = [];
    public List<ExamSubjectComponentDto> Components { get; set; } = [];
    public List<ExamTeacherAssignmentDto> TeacherAssignments { get; set; } = [];
    public List<ExamClassValidationDto> ClassValidations { get; set; } = [];
    public ExamStatisticsDto Statistics { get; set; } = new();
    public ExamReadinessBreakdownDto ReadinessBreakdown { get; set; } = new();
}

public class ExamClassHierarchyItemDto
{
    public int ClassId { get; set; }
    public string ClassName { get; set; } = "";
    public bool IsGroupBased { get; set; }
    public int ClassSortOrder { get; set; }
    public int? SectionId { get; set; }
    public string? SectionName { get; set; }
    public int? ParentSectionId { get; set; }
    public int? StudentGroupId { get; set; }
    public string? StudentGroupName { get; set; }
    public string? StudentGroupCode { get; set; }
    public int? ClassSubjectId { get; set; }
    public int? SubjectId { get; set; }
    public string? SubjectCode { get; set; }
    public string? SubjectName { get; set; }
    public string? SubjectNameBn { get; set; }
    public string? SubjectCategory { get; set; }
    public string? SubjectGroupName { get; set; }
    public bool IsMandatory { get; set; }
    public bool IsOptional { get; set; }
    public bool IsReligionSubject { get; set; }
    public bool IsPractical { get; set; }
    public string? ReligionType { get; set; }
    public decimal DefaultFullMarks { get; set; }
    public decimal DefaultPassMarks { get; set; }
    public decimal TheoryMarks { get; set; }
    public decimal PracticalMarks { get; set; }
    public decimal Credit { get; set; }
    public string? NctbCode { get; set; }
    public decimal? ClassSubjectFullMarks { get; set; }
    public decimal? ClassSubjectPassMarks { get; set; }
    public bool ClassSubjectIsOptional { get; set; }
}

public class ExamSubjectComponentDto
{
    public int SubjectMarkStructureId { get; set; }
    public int SubjectId { get; set; }
    public int? ClassId { get; set; }
    public int? StudentGroupId { get; set; }
    public int ComponentId { get; set; }
    public string ComponentCode { get; set; } = "";
    public string ComponentName { get; set; } = "";
    public string? ComponentDescription { get; set; }
    public bool IsPractical { get; set; }
    public bool ComponentIsOptional { get; set; }
    public int ComponentDisplayOrder { get; set; }
    public decimal DefaultFullMarks { get; set; }
    public decimal DefaultPassMarks { get; set; }
    public decimal FullMarks { get; set; }
    public decimal PassMarks { get; set; }
    public int StructureDisplayOrder { get; set; }
    public bool StructureIsActive { get; set; }
}

public class ExamTeacherAssignmentDto
{
    public int TeacherId { get; set; }
    public int SubjectId { get; set; }
    public int ClassId { get; set; }
    public int? SectionId { get; set; }
    public int? StudentGroupId { get; set; }
    public int AcademicYearId { get; set; }
    public int? EmployeeId { get; set; }
    public string? EmployeeCode { get; set; }
    public string? TeacherName { get; set; }
    public string? TeacherEmail { get; set; }
    public bool IsMissingTeacher { get; set; }
}

public class ExamClassValidationDto
{
    public int ClassId { get; set; }
    public string ClassName { get; set; } = "";
    public int SectionCount { get; set; }
    public int SubjectCount { get; set; }
    public int ComponentCount { get; set; }
    public int TeacherCount { get; set; }
    public int MissingTeacherCount { get; set; }
    public string ValidationStatus { get; set; } = "";
    public bool IsReady { get; set; }
}

public class ExamStatisticsDto
{
    public int StudentCount { get; set; }
    public int SectionCount { get; set; }
    public int SubjectCount { get; set; }
    public int ComponentCount { get; set; }
    public int TeacherCount { get; set; }
    public int TotalClasses { get; set; }
    public int TotalSections { get; set; }
    public int TotalSubjects { get; set; }
    public int TotalComponents { get; set; }
    public int TotalTeachersAssigned { get; set; }
}

public class ExamClassHierarchyDto
{
    public List<ExamClassHierarchyItemDto> Items { get; set; } = [];
}

public class ExamValidationResultDto
{
    public List<ExamValidationMessageDto> Messages { get; set; } = [];
    public int TotalClasses { get; set; }
    public int ReadyClasses { get; set; }
    public int NotReadyClasses { get; set; }
    public decimal ReadinessPercentage { get; set; }
    public int ErrorCount { get; set; }
    public int WarningCount { get; set; }
    public bool Is100PercentReady { get; set; }
    public List<ExamReadinessCategoryDto> ReadinessCategories { get; set; } = [];
}

public class ExamValidationMessageDto
{
    public string Severity { get; set; } = "";
    public string Category { get; set; } = "";
    public string Message { get; set; } = "";
    public int? ClassId { get; set; }
    public int? SectionId { get; set; }
    public int? SubjectId { get; set; }
    public string FixAction { get; set; } = "";
}

public class ExamReadinessCategoryDto
{
    public string Category { get; set; } = "";
    public string Label { get; set; } = "";
    public bool IsReady { get; set; }
}

public class ExamReadinessBreakdownDto
{
    public int TotalClasses { get; set; }
    public int ReadyClasses { get; set; }
    public int NotReadyClasses { get; set; }
    public decimal ReadinessPercentage { get; set; }
    public int ErrorCount { get; set; }
    public int WarningCount { get; set; }
    public bool Is100PercentReady { get; set; }
    public List<ExamReadinessCategoryDto> Categories { get; set; } = [];
    public List<ExamClassValidationDto> ClassValidations { get; set; } = [];
}

public class ExamCreateResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";
    public int ExamId { get; set; }
    public List<int> CreatedExamIds { get; set; } = [];
    public List<string> CreatedExamNames { get; set; } = [];
    public int SubjectCount { get; set; }
    public int TeacherAssignmentCount { get; set; }
}

public class ExamCreationReadinessDto
{
    public int TotalClasses { get; set; }
    public int TotalSections { get; set; }
    public int TotalSubjects { get; set; }
    public int TotalComponents { get; set; }
    public int TeachersAssigned { get; set; }
    public int TeachersMissing { get; set; }
    public decimal ReadinessPercentage { get; set; }
}

public class ExamScheduleResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";
    public int ScheduledCount { get; set; }
    public int ConflictsCount { get; set; }
}

public class ExamConflictDto
{
    public string ConflictType { get; set; } = "";
    public string Description { get; set; } = "";
    public int ExamSubjectId1 { get; set; }
    public int ExamSubjectId2 { get; set; }
    public int? TeacherId { get; set; }
    public int? RoomId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
}

public class ExamCreateHierarchyRequest
{
    public int AcademicYearId { get; set; }
    public string ExamName { get; set; } = "";
    public int ExamTerm { get; set; }
    public string ExamType { get; set; } = "";
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public List<int> ClassIds { get; set; } = [];
    public string SubjectsJson { get; set; } = "[]";
    public string UserId { get; set; } = "";
}

public class ExamPublishReadinessDto
{
    public bool IsReady { get; set; }
    public List<string> Blockers { get; set; } = [];
}

public class ExamFixResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";
    public int AssignmentId { get; set; }
    public int CreatedCount { get; set; }
}