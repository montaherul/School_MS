namespace SchoolManagementSystem.Models.DTOs.Academic;

public class AcademicDashboardDto
{
    // Core KPIs
    public int TotalStudents { get; set; }
    public int TotalTeachers { get; set; }
    public int TotalClasses { get; set; }
    public int TotalSections { get; set; }
    public int TotalSubjects { get; set; }
    public int ActiveAcademicYears { get; set; }
    public int CalendarEventsToday { get; set; }

    // Derived KPIs
    public int StudentTeacherRatio { get; set; }
    public double CapacityUtilizationPercent { get; set; }
    public int TotalRoutines { get; set; }
    public int TotalClassrooms { get; set; }
    public int ActiveGroups { get; set; }

    // Syllabus KPIs
    public int SyllabusTotal { get; set; }
    public int SyllabusCompleted { get; set; }
    public int SyllabusPending { get; set; }
    public double SyllabusCompletionPercent { get; set; }

    // Calendar KPIs
    public int UpcomingExams { get; set; }
    public int UpcomingHolidays { get; set; }
    public int TodayClasses { get; set; }
    public double TeacherLoadAverage { get; set; }

    // Lists
    public List<UpcomingExamItem> UpcomingExamList { get; set; } = [];
    public List<UpcomingHolidayItem> UpcomingHolidayList { get; set; } = [];
    public List<MonthlyTrendItem> MonthlyTrend { get; set; } = [];
    public List<StudentDistributionItem> StudentDistribution { get; set; } = [];
    public List<TeacherWorkloadItem> TeacherWorkload { get; set; } = [];
    public List<SubjectCategoryItem> SubjectCategories { get; set; } = [];
    public List<SectionCapacityItem> SectionCapacity { get; set; } = [];
    public List<ExamDistributionItem> ExamDistribution { get; set; } = [];
    public List<HolidayCalendarItem> HolidayCalendar { get; set; } = [];
}

public class UpcomingExamItem
{
    public string ExamName { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public string Subject { get; set; } = string.Empty;
}

public class UpcomingHolidayItem
{
    public string Name { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public string HolidayType { get; set; } = string.Empty;
}

public class MonthlyTrendItem
{
    public string Month { get; set; } = string.Empty;
    public int WorkingDays { get; set; }
    public int Holidays { get; set; }
    public int ExamDays { get; set; }
}

public class StudentDistributionItem
{
    public string ClassName { get; set; } = string.Empty;
    public int StudentCount { get; set; }
}

public class TeacherWorkloadItem
{
    public string TeacherName { get; set; } = string.Empty;
    public int SubjectCount { get; set; }
    public int ClassCount { get; set; }
    public int TotalPeriods { get; set; }
}

public class SubjectCategoryItem
{
    public string Category { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class SectionCapacityItem
{
    public string SectionName { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public int Occupied { get; set; }
    public double UtilizationPercent { get; set; }
}

public class ExamDistributionItem
{
    public string Month { get; set; } = string.Empty;
    public int ExamCount { get; set; }
}

public class HolidayCalendarItem
{
    public string Title { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public string HolidayType { get; set; } = string.Empty;
}
