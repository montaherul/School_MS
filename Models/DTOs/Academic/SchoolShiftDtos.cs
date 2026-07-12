using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models.DTOs.Academic;

public class SchoolShiftListItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public int TotalRecords { get; set; }
}

public class SchoolShiftUpsertDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(10)]
    public string Code { get; set; } = string.Empty;

    [Required]
    public TimeSpan StartTime { get; set; }

    [Required]
    public TimeSpan EndTime { get; set; }

    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}
