using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Entities.Base;
using SchoolManagementSystem.Models.Entities.Student;
using StudentEntity = SchoolManagementSystem.Models.Entities.Student.Student;

namespace SchoolManagementSystem.Models.Entities.Attendance;

public class AttendanceNotificationLog : BaseEntity
{
    public int StudentId { get; set; }
    public StudentEntity? Student { get; set; }

    public int? EmployeeId { get; set; }
    public SchoolManagementSystem.Models.Entities.Employee.Employee? Employee { get; set; }

    public int? GuardianId { get; set; }
    public SchoolManagementSystem.Models.Entities.Guardian.Guardian? Guardian { get; set; }

    public DateOnly AttendanceDate { get; set; }

    [MaxLength(160)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(60)]
    public string NotificationType { get; set; } = "Absent";

    public bool IsSent { get; set; }
    public DateTime? SentAt { get; set; }

    [MaxLength(1000)]
    public string? ErrorMessage { get; set; }

    [MaxLength(40)]
    public string NotificationChannel { get; set; } = "Email";

    [MaxLength(40)]
    public string NotificationStatus { get; set; } = "Pending";

    public int RetryCount { get; set; }

    public DateTime? NextRetryAt { get; set; }
}
