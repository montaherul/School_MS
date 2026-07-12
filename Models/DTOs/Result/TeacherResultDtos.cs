namespace SchoolManagementSystem.Models.DTOs.Result;

public class TeacherAssignedExamDto
{
    public int ExamId { get; set; }
    public string ExamName { get; set; } = "";
    public string Term { get; set; } = "";
    public DateOnly StartsOn { get; set; }
    public DateOnly EndsOn { get; set; }
    public int Status { get; set; }
    public int AcademicYearId { get; set; }
    public string AcademicYearName { get; set; } = "";
}

public class TeacherAssignedSubjectDto
{
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = "";
    public string SubjectCode { get; set; } = "";
    public int ClassId { get; set; }
    public string ClassName { get; set; } = "";
    public int SectionId { get; set; }
    public string SectionName { get; set; } = "";
    public int? GroupId { get; set; }
    public string GroupName { get; set; } = "";
}

public class TeacherMarksEntrySheetDto
{
    public bool Authorized { get; set; }
    public List<TeacherMarksEntryStudentDto> Students { get; set; } = new();
    public List<MarksEntryExistingDto> ExistingMarks { get; set; } = new();
}

public class TeacherMarksEntryStudentDto
{
    public int StudentId { get; set; }
    public string StudentNo { get; set; } = "";
    public string StudentName { get; set; } = "";
    public string RollNumber { get; set; } = "";
    public string ClassName { get; set; } = "";
    public string SectionName { get; set; } = "";
    public string GroupName { get; set; } = "";
}

public class MarksEntryExistingDto
{
    public int StudentId { get; set; }
    public decimal MarksObtained { get; set; }
    public string? Grade { get; set; }
    public decimal? GradePoint { get; set; }
    public ComponentMarksDto ComponentMarks { get; set; } = new();
    public string? ComponentValues { get; set; }
    public int Status { get; set; }
    public bool IsLocked { get; set; }
}

public class TeacherResultSummaryDto
{
    public int TotalStudents { get; set; }
    public int MarksEntered { get; set; }
    public int PassCount { get; set; }
    public int FailCount { get; set; }
    public decimal AvgMarks { get; set; }
    public decimal HighestMarks { get; set; }
    public decimal LowestMarks { get; set; }
    public List<GradeDistributionItemDto> GradeDistribution { get; set; } = new();
}

public class GradeDistributionItemDto
{
    public string Grade { get; set; } = "";
    public int Count { get; set; }
}

public class TeacherResultDashboardDto
{
    public int TeacherId { get; set; }
    public string TeacherName { get; set; } = "";
    public int TotalAssignedExams { get; set; }
    public int TotalAssignedSubjects { get; set; }
    public int PendingMarkEntries { get; set; }
    public int SubmittedMarkEntries { get; set; }
    public int TotalMarkEntries { get; set; }
    public double CompletionPercentage { get; set; }
    public List<TeacherPendingExamDto> PendingExams { get; set; } = [];
    public List<TeacherRecentActivityDto> RecentActivity { get; set; } = [];
    public List<TodayScheduleItemDto> TodaySchedule { get; set; } = [];
    public List<QuickActionItemDto> QuickActions { get; set; } = [];
    public List<NotificationItemDto> Notifications { get; set; } = [];
}

public class TodayScheduleItemDto
{
    public int EntryId { get; set; }
    public string PeriodName { get; set; } = "";
    public string StartTime { get; set; } = "";
    public string EndTime { get; set; } = "";
    public string SubjectName { get; set; } = "";
    public string ClassName { get; set; } = "";
    public string SectionName { get; set; } = "";
    public string RoomNo { get; set; } = "";
    public int DayNumber { get; set; }
}

public class QuickActionItemDto
{
    public string Label { get; set; } = "";
    public string Url { get; set; } = "";
    public string IconName { get; set; } = "";
    public string Color { get; set; } = "";
}

public class NotificationItemDto
{
    public string Title { get; set; } = "";
    public string Body { get; set; } = "";
    public bool IsRead { get; set; }
    public DateTime? SentAt { get; set; }
}

public class TeacherPendingExamDto
{
    public int ExamId { get; set; }
    public string ExamName { get; set; } = "";
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = "";
    public int ClassId { get; set; }
    public string ClassName { get; set; } = "";
    public int SectionId { get; set; }
    public string SectionName { get; set; } = "";
    public int? GroupId { get; set; }
    public string GroupName { get; set; } = "";
    public int StudentCount { get; set; }
    public int CompletedCount { get; set; }
    public string Status { get; set; } = "";
    public bool IsReadOnly { get; set; }
}

public class TeacherRecentActivityDto
{
    public string Action { get; set; } = "";
    public string Detail { get; set; } = "";
    public DateTime Timestamp { get; set; }
}

public class TeacherExportRowDto
{
    public string RollNumber { get; set; } = "";
    public string StudentNo { get; set; } = "";
    public string StudentName { get; set; } = "";
    public string ClassName { get; set; } = "";
    public string SectionName { get; set; } = "";
    public string GroupName { get; set; } = "";
    public decimal MarksObtained { get; set; }
    public ComponentMarksDto ComponentMarks { get; set; } = new();
    public string? Grade { get; set; }
    public decimal? GradePoint { get; set; }
    public string PassStatus { get; set; } = "";
    public int Status { get; set; }
}


