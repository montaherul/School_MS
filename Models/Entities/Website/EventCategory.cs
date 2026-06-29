using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Entities.Base;

namespace SchoolManagementSystem.Models.Entities.Website;

public class EventCategory : BaseEntity
{
    [MaxLength(100)]
    public string? Name { get; set; }

    [MaxLength(100)]
    public string? Slug { get; set; }

    [MaxLength(200)]
    public string? Description { get; set; }

    [MaxLength(7)]
    public string? ColorCode { get; set; }

    public int DisplayOrder { get; set; } = 0;

    public bool IsActive { get; set; } = true;
}