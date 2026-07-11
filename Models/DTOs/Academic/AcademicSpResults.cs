namespace SchoolManagementSystem.Models.DTOs.Academic;

public class AcademicYearSpResult
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartsOn { get; set; }
    public DateTime EndsOn { get; set; }
    public bool IsActive { get; set; }
    public int TotalRecords { get; set; }
}

public class ClassListSpResult
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int SortOrder { get; set; }
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
    public int StudentCount { get; set; }
    public int TotalRecords { get; set; }
}

public class SubjectListSpResult
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int TotalRecords { get; set; }
}
