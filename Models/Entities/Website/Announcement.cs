using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Entities.Base;

namespace SchoolManagementSystem.Models.Entities.Website;

public class Announcement : BaseEntity
{
    [MaxLength(200)]
    public string? Title { get; set; }

    [MaxLength(2000)]
    public string? Content { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime? PublishDate { get; set; }

    public DateTime? ExpiryDate { get; set; }

    [MaxLength(50)]
    public string? Priority { get; set; } = "Normal";
}