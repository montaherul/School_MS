using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Entities.Base;

namespace SchoolManagementSystem.Models.Entities.Website;

public class Slider : BaseEntity
{
    [MaxLength(200)]
    public string? Title { get; set; }

    [MaxLength(500)]
    public string? Subtitle { get; set; }

    [MaxLength(200)]
    public string? ButtonText { get; set; }

    [MaxLength(500)]
    public string? ButtonUrl { get; set; }

    [MaxLength(200)]
    public string? ImagePath { get; set; }

    public int DisplayOrder { get; set; } = 0;

    public bool IsActive { get; set; } = true;

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }
}