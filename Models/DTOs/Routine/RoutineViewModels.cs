namespace SchoolManagementSystem.Models.DTOs.Routine;

public class RoutineClassViewModel
{
    public List<AcademicYearItem> AcademicYears { get; set; } = new();
    public List<ClassItem> Classes { get; set; } = new();
}

public class AcademicYearItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class ClassItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class RoutineRoomViewModel
{
    public List<RoomItem> Rooms { get; set; } = new();
}

public class RoomItem
{
    public int Id { get; set; }
    public string RoomNo { get; set; } = string.Empty;
    public string? Name { get; set; }
}

public class RoutineTeacherViewModel
{
    public string TeacherName { get; set; } = string.Empty;
    public int TotalSubjects { get; set; }
    public int TotalPeriodsPerWeek { get; set; }
    public int TotalClasses { get; set; }
    public int TotalWorkingDays { get; set; }
    public List<object> WeeklyGrid { get; set; } = new();
    public List<TodayClassDto> TodayClasses { get; set; } = new();
}

public class RoutineStudentViewModel
{
    public string ClassName { get; set; } = string.Empty;
    public string? SectionName { get; set; }
    public string? GroupName { get; set; }
    public List<StatisticItem> Statistics { get; set; } = new();
    public List<object> WeeklyGrid { get; set; } = new();
    public List<TodayStudentClassDto> TodayClasses { get; set; } = new();
}

public class TodayClassDto
{
    public string PeriodName { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string SectionName { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public string RoomNo { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
}

public class TodayStudentClassDto
{
    public string PeriodName { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public string TeacherName { get; set; } = string.Empty;
    public string RoomNo { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
}

public class StatisticItem
{
    public string IconClass { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public int Value { get; set; }
    public string Label { get; set; } = string.Empty;
}

public class SectionItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class RoutineSettingsViewModel
{
    public int MaxTeacherPeriodsPerDay { get; set; } = 7;
    public int MaxTeacherPeriodsPerWeek { get; set; } = 35;
    public bool AutoPublishAfterGeneration { get; set; } = false;
    public bool EnableConflictDetection { get; set; } = true;
    public string GenerationAlgorithmVersion { get; set; } = "V1";
    public int WorkingDaysPerWeek { get; set; } = 6;
}

public record TeacherLookupDto(int Id, string Name);
public record SubjectLookupDto(int Id, string Name);
public record PeriodLookupDto(int Id, string Name, string StartTime, string EndTime);
public record RoutineEntryLookupDto(int Id, string Display);
public record GroupLookupDto(int Id, string Name);
