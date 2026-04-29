using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models.DTOs.Academic;

public class AcademicYearListItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string StartsOn { get; set; } = string.Empty;
    public string EndsOn { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class AcademicYearUpsertDto
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(30)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public DateTime StartsOn { get; set; }

    [Required]
    public DateTime EndsOn { get; set; }

    public bool IsActive { get; set; }
}
