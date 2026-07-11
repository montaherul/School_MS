using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models.DTOs.Academic;

#pragma warning disable CS8618

public class StudentGroupListItemDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
    public int MinClass { get; set; }
    public int MaxClass { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public int TotalRecords { get; set; }
}

public class StudentGroupUpsertDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(50)]
    public string Code { get; set; } = string.Empty;

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [Range(1, 12)]
    public int MinClass { get; set; } = 9;

    [Range(1, 12)]
    public int MaxClass { get; set; } = 10;

    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}
