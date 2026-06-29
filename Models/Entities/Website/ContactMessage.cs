using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Entities.Base;

namespace SchoolManagementSystem.Models.Entities.Website;

public class ContactMessage : BaseEntity
{
    [MaxLength(200)]
    public string? Name { get; set; }

    [MaxLength(200)]
    public string? Email { get; set; }

    [MaxLength(20)]
    public string? Phone { get; set; }

    [MaxLength(200)]
    public string? Subject { get; set; }

    [MaxLength(1000)]
    public string? Message { get; set; }

    [MaxLength(50)]
    public string? Status { get; set; } = "Unread";
}