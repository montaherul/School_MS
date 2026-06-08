using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models.DTOs.Academic;

public class SchoolClassListItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public bool IsGroupBased { get; set; }
    public int SectionCount { get; set; }
    public int StudentCount { get; set; }
    public int TotalRecords { get; set; }
}

public class SchoolClassUpsertDto
{
    public int Id { get; set; }
    [Required]
    [StringLength(60)]
    public string Name { get; set; } = string.Empty;
    [Required]
    public int SortOrder { get; set; }
    public bool IsGroupBased { get; set; }
}

