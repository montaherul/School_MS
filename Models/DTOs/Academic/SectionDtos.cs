using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models.DTOs.Academic;

public class SectionListItemDto
{
    public int Id { get; set; }
    public int SchoolClassId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public int? ParentSectionId { get; set; }
    public int? StudentGroupId { get; set; }
    public string? GroupName { get; set; }   // Parent group name for Class 9/10
    public int TotalRecords { get; set; }
}

public class SectionUpsertDto
{
    public int Id { get; set; }
    [Required]
    public int SchoolClassId { get; set; }
    [Required]
    [StringLength(20)]
    public string Name { get; set; } = string.Empty;
    public int? ParentSectionId { get; set; }  // optional parent group
}

/// <summary>Used in the Approve modal — leaf sections grouped by their parent group.</summary>
public class SectionOptionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;   // "Science", "Business Studies" etc.
    public int StudentCount { get; set; }
    public int Capacity { get; set; }
    public bool IsFull => StudentCount >= Capacity;
}
