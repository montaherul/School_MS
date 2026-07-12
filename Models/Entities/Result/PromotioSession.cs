using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Entities.Base;

namespace SchoolManagementSystem.Models.Entities.Result;

public class PromotioSession : BaseEntity
{
    public int AcademicYearId { get; set; }
    public Academic.AcademicYear? AcademicYear { get; set; }

    [MaxLength(200)]
    public string SessionName { get; set; } = string.Empty;

    public DateTime PromotionDate { get; set; } = DateTime.UtcNow;

    [MaxLength(20)]
    public string Status { get; set; } = "Draft";

    [MaxLength(500)]
    public string? Remarks { get; set; }

    public bool IsRollbackAllowed => Status == "Draft";

    public int? ExecutedByUserId { get; set; }
    public DateTime? ExecutedAt { get; set; }
    public int? ApprovedByUserId { get; set; }
    public DateTime? ApprovedAt { get; set; }

    public virtual ICollection<PromotionHistory> Promotions { get; set; } = [];
}
