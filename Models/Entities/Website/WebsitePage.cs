using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Entities.Base;

namespace SchoolManagementSystem.Models.Entities.Website;

public class WebsitePage : BaseEntity
{
    [MaxLength(200)]
    public string? Title { get; set; }

    [MaxLength(200)]
    public string? Slug { get; set; }

    public string? Content { get; set; }

    [MaxLength(200)]
    public string? MetaTitle { get; set; }

    [MaxLength(500)]
    public string? MetaDescription { get; set; }

    public bool IsPublished { get; set; } = true;

    public int DisplayOrder { get; set; } = 0;

    public DateTime? PublishAt { get; set; }
}