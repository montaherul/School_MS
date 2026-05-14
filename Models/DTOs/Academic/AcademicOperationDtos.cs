namespace SchoolManagementSystem.Models.DTOs.Academic;

public class TeacherSubjectAssignmentDto
{
    public int Id { get; set; }
    public long EmployeeId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public int ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public int SectionId { get; set; }
    public string SectionName { get; set; } = string.Empty;
    public bool IsClassTeacher { get; set; }
}

public class ClassRoutineDto
{
    public int Id { get; set; }
    public int ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public int SectionId { get; set; }
    public string SectionName { get; set; } = string.Empty;
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public long EmployeeId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string? RoomNo { get; set; }
}

public class TeacherWorkloadDto
{
    public long EmployeeId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public int TotalSubjects { get; set; }
    public int TotalClasses { get; set; }
    public int WeeklyPeriods { get; set; }
    public int PendingMarkEntries { get; set; }
}

public class AcademicDocumentDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string UploadedByName { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
}
