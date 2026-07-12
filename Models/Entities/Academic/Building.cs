using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Entities.Base;
namespace SchoolManagementSystem.Models.Entities.Academic;

public class Building : BaseEntity
{
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Code { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Address { get; set; }

    public int TotalFloors { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;
}
