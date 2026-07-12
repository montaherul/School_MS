namespace SchoolManagementSystem.Models.DTOs.Academic;

public class AcademicYearSpResult
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public DateTime StartsOn { get; set; }
    public DateTime EndsOn { get; set; }
    public bool IsActive { get; set; }
    public bool IsCurrent { get; set; }
    public bool IsLocked { get; set; }
    public string Status { get; set; } = string.Empty;
    public int TotalRecords { get; set; }
}

public class ClassListSpResult
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameBn { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public int Capacity { get; set; }
    public bool IsGroupBased { get; set; }
    public bool IsHigherSecondary { get; set; }
    public bool IsActive { get; set; }
    public int SectionCount { get; set; }
    public int StudentCount { get; set; }
    public int TotalRecords { get; set; }
}

public class SectionListSpResult
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int SchoolClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public int? ParentSectionId { get; set; }
    public int? StudentGroupId { get; set; }
    public string? GroupName { get; set; }
    public int StudentCount { get; set; }
    public int Capacity { get; set; }
    public int TotalRecords { get; set; }
}

public class SubjectListSpResult
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string NameBn { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public bool IsMandatory { get; set; }
    public bool IsOptional { get; set; }
    public bool IsPractical { get; set; }
    public decimal TheoryMarks { get; set; }
    public decimal PracticalMarks { get; set; }
    public decimal PassMarks { get; set; }
    public decimal Credit { get; set; }
    public string? NctbCode { get; set; }
    public bool IsActive { get; set; }
    public int TotalRecords { get; set; }
}

public class TeacherAssignmentSpResult
{
    public int Id { get; set; }
    public int TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public string EmployeeCode { get; set; } = string.Empty;
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public int ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public int SectionId { get; set; }
    public string SectionName { get; set; } = string.Empty;
    public int? GroupId { get; set; }
    public string? GroupName { get; set; }
    public int AcademicYearId { get; set; }
    public string AcademicYearName { get; set; } = string.Empty;
    public bool IsClassTeacher { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ClassSubjectSpResult
{
    public int Id { get; set; }
    public int ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public int SubjectId { get; set; }
    public string SubjectCode { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public bool IsMandatory { get; set; }
    public bool IsOptional { get; set; }
    public bool IsPractical { get; set; }
    public decimal TheoryMarks { get; set; }
    public decimal PracticalMarks { get; set; }
    public decimal PassMarks { get; set; }
    public int? GroupId { get; set; }
    public string? GroupName { get; set; }
    public int TotalRecords { get; set; }
}

public class AcademicDashboardSpResult
{
    public int TotalClasses { get; set; }
    public int TotalSections { get; set; }
    public int TotalSubjects { get; set; }
    public int TotalTeachers { get; set; }
    public int TotalStudents { get; set; }
    public int TotalRooms { get; set; }
    public int TotalSessions { get; set; }
    public int TotalShifts { get; set; }
    public int TotalBuildings { get; set; }
    public int ActiveAcademicYears { get; set; }
    public int ScienceGroupCount { get; set; }
    public int BusinessGroupCount { get; set; }
    public int HumanitiesGroupCount { get; set; }
}

public class NctbComplianceSpResult
{
    public int AcademicYearId { get; set; }
    public string AcademicYearName { get; set; } = string.Empty;
    public int CoreSubjectCount { get; set; }
    public int ElectiveSubjectCount { get; set; }
    public int VocationalSubjectCount { get; set; }
    public int ReligionSubjectCount { get; set; }
    public int TotalSubjectCount { get; set; }
    public int GroupCount { get; set; }
    public int TotalClassCount { get; set; }
    public int MappedClassCount { get; set; }
    public bool HasScienceGroup { get; set; }
    public bool HasBusinessStudiesGroup { get; set; }
    public bool HasHumanitiesGroup { get; set; }
    public bool HasCompulsoryCoreSubjects { get; set; }
    public bool HasAllReligionTypes { get; set; }
    public bool HasPrimaryClasses { get; set; }
    public bool HasSecondaryClasses { get; set; }
    public bool HasIslamicStudies { get; set; }
    public string CoreSubjectNames { get; set; } = string.Empty;
    public string ElectiveSubjectNames { get; set; } = string.Empty;
    public string VocationalSubjectNames { get; set; } = string.Empty;
    public string ReligionSubjectNames { get; set; } = string.Empty;
}
