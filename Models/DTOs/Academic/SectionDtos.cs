using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models.DTOs.Academic;

public class SectionListItemDto
{
    public int Id { get; set; }
    public int SchoolClassId { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class SectionUpsertDto
{
    public int Id { get; set; }
    [Required]
    public int SchoolClassId { get; set; }
    [Required]
    [StringLength(20)]
    public string Name { get; set; } = string.Empty;
}

