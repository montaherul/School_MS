using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models.DTOs.Academic;

public class BuildingListItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Address { get; set; }
    public int TotalFloors { get; set; }
    public bool IsActive { get; set; }
    public int TotalRecords { get; set; }
}

public class BuildingUpsertDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string Code { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Address { get; set; }

    public int TotalFloors { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    public bool IsActive { get; set; }
}
