using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models.DTOs.Academic;
#pragma warning disable CS8618

public class AcademicYearReportDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime StartsOn { get; set; }
    public DateTime EndsOn { get; set; }
    public bool IsActive { get; set; }
    public int ClassCount { get; set; }
    public int SectionCount { get; set; }
    public int StudentCount { get; set; }
    public int SubjectCount { get; set; }
}

public class ClassReportDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public int SectionCount { get; set; }
    public int StudentCount { get; set; }
    public int Capacity { get; set; }
    public double OccupancyPercent { get; set; }
    public int SubjectCount { get; set; }
    public bool IsActive { get; set; }
}

public class SectionReportDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string ClassName { get; set; }
    public string GroupName { get; set; }
    public int Capacity { get; set; }
    public int Occupied { get; set; }
    public double OccupancyPercent { get; set; }
    public bool IsActive { get; set; }
}

public class SubjectReportDto
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }
    public bool IsMandatory { get; set; }
    public bool IsPractical { get; set; }
    public bool IsActive { get; set; }
    public int ClassCount { get; set; }
    public int TeacherCount { get; set; }
}

public class TeacherLoadReportDto
{
    public int TeacherId { get; set; }
    public string TeacherName { get; set; }
    public string SubjectNames { get; set; }
    public int AssignedClasses { get; set; }
    public int AssignedSections { get; set; }
    public int TotalPeriodsPerWeek { get; set; }
}

public class SyllabusProgressReportDto
{
    public int SyllabusId { get; set; }
    public string Title { get; set; }
    public string ClassName { get; set; }
    public string SubjectName { get; set; }
    public string AcademicYear { get; set; }
    public bool IsActive { get; set; }
    public string UploadedBy { get; set; }
    public DateTime UploadedAt { get; set; }
}

public class StudentDistributionReportDto
{
    public string ClassName { get; set; }
    public string SectionName { get; set; }
    public string GroupName { get; set; }
    public int StudentCount { get; set; }
}

public class CapacityReportDto
{
    public string ClassName { get; set; }
    public int TotalCapacity { get; set; }
    public int TotalOccupied { get; set; }
    public int AvailableSeats { get; set; }
    public double OccupancyPercent { get; set; }
}

public class AcademicReportFilterDto
{
    public string ReportType { get; set; } = "academic-year";
    public int? AcademicYearId { get; set; }
    public int? ClassId { get; set; }
    public int? SectionId { get; set; }
    public int? SubjectId { get; set; }
    public int? TeacherId { get; set; }
}

public class AcademicReportViewModel
{
    public AcademicReportFilterDto Filter { get; set; } = new();
    public List<AcademicYearReportDto>? AcademicYearReports { get; set; }
    public List<ClassReportDto>? ClassReports { get; set; }
    public List<SectionReportDto>? SectionReports { get; set; }
    public List<SubjectReportDto>? SubjectReports { get; set; }
    public List<TeacherLoadReportDto>? TeacherLoadReports { get; set; }
    public List<SyllabusProgressReportDto>? SyllabusReports { get; set; }
    public List<StudentDistributionReportDto>? StudentDistribution { get; set; }
    public List<CapacityReportDto>? CapacityReports { get; set; }
}
