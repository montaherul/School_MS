using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Entities.Base;
using SchoolManagementSystem.Models.Entities.Student;
using SchoolManagementSystem.Models.Enums;
using StudentEntity = SchoolManagementSystem.Models.Entities.Student.Student;

namespace SchoolManagementSystem.Models.Entities.Guardian;

public enum GuardianRelationshipType
{
    Father = 1,
    Mother = 2,
    LegalGuardian = 3,
    Grandfather = 4,
    Grandmother = 5,
    Uncle = 6,
    Aunt = 7,
    Brother = 8,
    Sister = 9,
    Other = 10
}

public enum GuardianStatus
{
    Active = 1,
    Inactive = 2,
    PendingActivation = 3
}

public class Guardian : BaseEntity
{
    [MaxLength(30)]
    public string GuardianCode { get; set; } = string.Empty;

    [MaxLength(80)]
    public string FirstName { get; set; } = string.Empty;

    [MaxLength(80)]
    public string LastName { get; set; } = string.Empty;

    [MaxLength(160)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Gender { get; set; } = string.Empty;

    public DateTime? DateOfBirth { get; set; }

    public GuardianRelationshipType RelationType { get; set; }

    [MaxLength(50)]
    public string? NationalId { get; set; }

    [MaxLength(50)]
    public string? PassportNumber { get; set; }

    [MaxLength(100)]
    public string? Occupation { get; set; }

    [MaxLength(150)]
    public string? EmployerName { get; set; }

    public decimal? MonthlyIncome { get; set; }

    [Required]
    [MaxLength(30)]
    public string MobileNumber { get; set; } = string.Empty;

    [MaxLength(30)]
    public string? AlternativeMobileNumber { get; set; }

    [MaxLength(160)]
    public string? Email { get; set; }

    [MaxLength(250)]
    public string? PresentAddress { get; set; }

    [MaxLength(250)]
    public string? PermanentAddress { get; set; }

    [MaxLength(260)]
    public string? PhotoPath { get; set; }

    [MaxLength(100)]
    public string? EmergencyContactName { get; set; }

    [MaxLength(30)]
    public string? EmergencyContactNumber { get; set; }

    public bool PortalAccessEnabled { get; set; } = false;

    public bool ReceiveEmailNotifications { get; set; } = true;

    public bool ReceiveSMSNotifications { get; set; } = true;

    public bool ReceiveEventNotifications { get; set; } = true;

    public bool ReceiveFeeNotifications { get; set; } = true;

    public bool ReceiveExamNotifications { get; set; } = true;

    public bool ReceiveAttendanceNotifications { get; set; } = true;

    public bool IsPrimaryGuardian { get; set; } = false;

    public GuardianStatus Status { get; set; } = GuardianStatus.PendingActivation;

    [MaxLength(500)]
    public string? Remarks { get; set; }

    public int? UserId { get; set; }
    
    public ICollection<StudentGuardian> StudentGuardians { get; set; } = new List<StudentGuardian>();
}

public class StudentGuardian : BaseEntity
{
    public int StudentId { get; set; }
    public StudentEntity? Student { get; set; }

    public int GuardianId { get; set; }
    public Guardian? Guardian { get; set; }

    public GuardianRelationshipType Relationship { get; set; }

    public bool IsPrimaryGuardian { get; set; } = false;

    public bool ReceivesAttendanceNotifications { get; set; } = true;
    public bool ReceivesResultNotifications { get; set; } = true;
    public bool ReceivesFeeNotifications { get; set; } = true;
    public bool ReceivesSMS { get; set; } = true;
    public bool ReceivesEmail { get; set; } = true;
    public bool ReceivesWhatsApp { get; set; } = false;
}

public class GuardianNotification : BaseEntity
{
    public int GuardianId { get; set; }
    public Guardian? Guardian { get; set; }

    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public bool IsRead { get; set; } = false;
    public DateTime? ReadAt { get; set; }
    
    [MaxLength(50)]
    public string? Category { get; set; } // Attendance, Result, Fee, etc.
}

public class GuardianNotificationLog : BaseEntity
{
    public int GuardianId { get; set; }
    public Guardian? Guardian { get; set; }

    [MaxLength(50)]
    public string Channel { get; set; } = string.Empty; // SMS, Email, etc.

    [MaxLength(160)]
    public string Recipient { get; set; } = string.Empty;

    public string MessageContent { get; set; } = string.Empty;

    public bool IsSent { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}
