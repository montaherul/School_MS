using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Entities.Base;

namespace SchoolManagementSystem.Models.Entities.Website;

public class SchoolSetting : BaseEntity
{
    [MaxLength(160)]
    public string SchoolName { get; set; } = string.Empty;

    [MaxLength(80)]
    public string ShortName { get; set; } = string.Empty;

    [MaxLength(20)]
    public string EIIN { get; set; } = string.Empty;

    [MaxLength(300)]
    public string Address { get; set; } = string.Empty;

    [MaxLength(30)]
    public string Phone { get; set; } = string.Empty;

    [MaxLength(160)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(260)]
    public string Website { get; set; } = string.Empty;

    [MaxLength(260)]
    public string? FacebookUrl { get; set; }

    [MaxLength(260)]
    public string? YouTubeUrl { get; set; }

    [MaxLength(260)]
    public string? LogoPath { get; set; }

    [MaxLength(260)]
    public string? FaviconPath { get; set; }

    [MaxLength(160)]
    public string? PrincipalName { get; set; }

    [MaxLength(4000)]
    public string? PrincipalMessage { get; set; }

    [MaxLength(260)]
    public string? PrincipalImagePath { get; set; }

    [MaxLength(2000)]
    public string? Mission { get; set; }

    [MaxLength(2000)]
    public string? Vision { get; set; }

    [MaxLength(500)]
    public string? FooterText { get; set; }

    [MaxLength(1000)]
    public string? GoogleMapEmbed { get; set; }
}

public class WebsitePage : BaseEntity
{
    [MaxLength(160)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(160)]
    public string Slug { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    [MaxLength(160)]
    public string? MetaTitle { get; set; }

    [MaxLength(260)]
    public string? MetaDescription { get; set; }

    public bool IsPublished { get; set; } = true;
}

public class Slider : BaseEntity
{
    [MaxLength(160)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(260)]
    public string? Subtitle { get; set; }

    [MaxLength(50)]
    public string? ButtonText { get; set; }

    [MaxLength(260)]
    public string? ButtonUrl { get; set; }

    [MaxLength(260)]
    public string ImagePath { get; set; } = string.Empty;

    public int DisplayOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;
}

public class Event : BaseEntity
{
    [MaxLength(160)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(4000)]
    public string Description { get; set; } = string.Empty;

    public DateTime EventDate { get; set; } = DateTime.UtcNow;

    [MaxLength(160)]
    public string? EventLocation { get; set; }

    [MaxLength(260)]
    public string? CoverImagePath { get; set; }

    public bool IsUpcoming { get; set; } = true;
    public bool IsPublished { get; set; } = true;
}

public class Gallery : BaseEntity
{
    [MaxLength(160)]
    public string AlbumName { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(260)]
    public string? CoverImagePath { get; set; }

    public virtual ICollection<GalleryImage> Images { get; set; } = new List<GalleryImage>();
}

public class GalleryImage : BaseEntity
{
    public int GalleryId { get; set; }
    public virtual Gallery? Gallery { get; set; }

    [MaxLength(260)]
    public string ImagePath { get; set; } = string.Empty;

    [MaxLength(260)]
    public string? Caption { get; set; }

    public int DisplayOrder { get; set; } = 0;
}

public class Announcement : BaseEntity
{
    [MaxLength(260)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string Content { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}
