using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models.DTOs.Academic;

public class SubjectCategoryListItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public int TotalRecords { get; set; }
}

public class SubjectCategoryUpsertDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string Code { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Description { get; set; }

    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}
