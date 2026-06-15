using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Entities.Base;

namespace SchoolManagementSystem.Models.Entities.Academic;

public class HolidayMaster : BaseEntity
{
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? NameBn { get; set; }

    [MaxLength(100)]
    public string HolidayType { get; set; } = string.Empty;

    public DateOnly HolidayDate { get; set; }

    public bool IsRecurring { get; set; }

    [MaxLength(50)]
    public string? Religion { get; set; }

    [MaxLength(10)]
    public string CountryCode { get; set; } = "BD";

    [MaxLength(500)]
    public string? Description { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; } = true;
}
