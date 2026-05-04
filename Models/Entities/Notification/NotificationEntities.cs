using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Entities.Base;
using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Models.Entities.Notification;

public class NotificationMessage : BaseEntity
{
    public int? UserId { get; set; }
    public NotificationChannel Channel { get; set; }

    [MaxLength(160)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string Body { get; set; } = string.Empty;

    public bool IsRead { get; set; }
    public DateTime? SentAt { get; set; }
}
