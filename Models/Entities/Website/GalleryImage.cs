using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Entities.Base;

namespace SchoolManagementSystem.Models.Entities.Website;

public class GalleryImage : BaseEntity
{
    [MaxLength(200)]
    public string? ImagePath { get; set; }

    [MaxLength(200)]
    public string? AltText { get; set; }

    [MaxLength(500)]
    public string? Caption { get; set; }

    public int DisplayOrder { get; set; } = 0;

    public int? GalleryId { get; set; }
    public Gallery? Gallery { get; set; }
}