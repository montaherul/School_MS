namespace SchoolManagementSystem.Models.DTOs.Routine;

public class RoutineAnalyticsViewModel
{
    public List<TeacherLoadDto> TeacherLoadSummary { get; set; } = new();
    public List<RoomUtilizationDto> RoomUtilization { get; set; } = new();
    public List<SubjectDistributionDto> SubjectDistribution { get; set; } = new();
    public int TotalConflicts { get; set; }
    public int TeacherConflicts { get; set; }
    public int RoomConflicts { get; set; }
    public int StudentConflicts { get; set; }
}

public class SubjectDistributionDto
{
    public string SubjectName { get; set; } = string.Empty;
    public int TotalPeriods { get; set; }
}
