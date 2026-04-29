using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models.Entities.Base;

public abstract class BaseEntity
{
    public int Id { get; set; }

    [MaxLength(64)]
    public string CreatedBy { get; set; } = "system";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(64)]
    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }
}
