using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using SchoolManagementSystem.Models.Entities.Base;

namespace SchoolManagementSystem.Models.Entities.Website;

public class Gallery : BaseEntity
{
    [MaxLength(200)]
    public string? AlbumName { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(200)]
    public string? CoverImagePath { get; set; }

    public int DisplayOrder { get; set; } = 0;

    public bool IsPublished { get; set; } = true;

    public ICollection<GalleryImage> Images { get; set; } = new List<GalleryImage>();
}