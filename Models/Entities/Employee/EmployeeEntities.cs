using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagementSystem.Models.Entities.Employee;

public class Employee
{
    [Key]
    public long Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string EmployeeCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Gender { get; set; } = string.Empty;

    [Required]
    public DateTime DateOfBirth { get; set; }

    [Required]
    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;

    [MaxLength(150)]
    [EmailAddress]
    public string? Email { get; set; }

    [MaxLength(10)]
    public string? BloodGroup { get; set; }

    [MaxLength(50)]
    public string Nationality { get; set; } = "Bangladeshi";

    // Present Address
    [MaxLength(150)]
    public string? PresentVillage { get; set; }

    [MaxLength(150)]
    public string? PresentPostOffice { get; set; }

    [MaxLength(150)]
    public string? PresentThana { get; set; }

    [MaxLength(100)]
    public string? PresentDistrict { get; set; }

    // Permanent Address
    [MaxLength(150)]
    public string? PermanentVillage { get; set; }

    [MaxLength(150)]
    public string? PermanentPostOffice { get; set; }

    [MaxLength(150)]
    public string? PermanentThana { get; set; }

    [MaxLength(100)]
    public string? PermanentDistrict { get; set; }

    [Required]
    public DateTime JoiningDate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Salary { get; set; }

    [MaxLength(500)]
    public string? PhotoPath { get; set; }

    public bool IsActive { get; set; } = true;

    [Required]
    public long DepartmentId { get; set; }
    
    [ForeignKey("DepartmentId")]
    public virtual Department Department { get; set; } = null!;

    [Required]
    public long DesignationId { get; set; }
    
    [ForeignKey("DesignationId")]
    public virtual Designation Designation { get; set; } = null!;

    public virtual SchoolManagementSystem.Models.Entities.Auth.ApplicationUser? User { get; set; }
}


public class Department
{
    [Key]
    public long Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Code { get; set; } = string.Empty;

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}

public class Designation
{
    [Key]
    public long Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}

public class EmployeeAttendance
{
    [Key]
    public long Id { get; set; }

    [Required]
    public long EmployeeId { get; set; }

    [ForeignKey("EmployeeId")]
    public virtual Employee Employee { get; set; } = null!;

    [Required]
    public DateTime AttendanceDate { get; set; }

    [Required]
    public SchoolManagementSystem.Models.Enums.AttendanceStatus Status { get; set; }

    public TimeSpan? CheckInTime { get; set; }
    public TimeSpan? CheckOutTime { get; set; }

    [MaxLength(500)]
    public string? Remarks { get; set; }

    [Required]
    [MaxLength(50)]
    public string CreatedBy { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(50)]
    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }
}

public class LeaveType
{
    [Key]
    public long Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public int DefaultDaysPerYear { get; set; }
    public bool IsPaid { get; set; } = true;

    [MaxLength(20)]
    public string? ColorCode { get; set; }

    public bool IsActive { get; set; } = true;
}

public class EmployeeLeave
{
    [Key]
    public long Id { get; set; }

    [Required]
    public long EmployeeId { get; set; }

    [ForeignKey("EmployeeId")]
    public virtual Employee Employee { get; set; } = null!;

    [Required]
    public long LeaveTypeId { get; set; }

    [ForeignKey("LeaveTypeId")]
    public virtual LeaveType LeaveType { get; set; } = null!;

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    public int TotalDays { get; set; }

    [Required]
    [MaxLength(500)]
    public string Reason { get; set; } = string.Empty;

    [Required]
    public SchoolManagementSystem.Models.Enums.LeaveStatus Status { get; set; } = SchoolManagementSystem.Models.Enums.LeaveStatus.Pending;

    public int? ApprovedById { get; set; }

    [ForeignKey("ApprovedById")]
    public virtual SchoolManagementSystem.Models.Entities.Auth.ApplicationUser? ApprovedBy { get; set; }

    public DateTime? ApprovedAt { get; set; }

    [MaxLength(500)]
    public string? RejectionReason { get; set; }

    [MaxLength(500)]
    public string? Remarks { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

public class EmployeeSalaryStructure
{
    [Key]
    public long Id { get; set; }

    [Required]
    public long EmployeeId { get; set; }

    [ForeignKey("EmployeeId")]
    public virtual Employee Employee { get; set; } = null!;

    [Column(TypeName = "decimal(18,2)")]
    public decimal BasicSalary { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal HouseRent { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal MedicalAllowance { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TransportAllowance { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal OtherAllowance { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TaxPercentage { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal ProvidentFund { get; set; }

    public DateTime EffectiveFrom { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class EmployeePayroll
{
    [Key]
    public long Id { get; set; }

    [Required]
    public long EmployeeId { get; set; }

    [ForeignKey("EmployeeId")]
    public virtual Employee Employee { get; set; } = null!;

    public int PayrollMonth { get; set; }
    public int PayrollYear { get; set; }

    public int WorkingDays { get; set; }
    public int PresentDays { get; set; }
    public int AbsentDays { get; set; }
    public int LeaveDays { get; set; }
    public int PaidLeaveDays { get; set; }
    public int UnpaidLeaveDays { get; set; }
    public int LateDays { get; set; }
    public double OvertimeHours { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal BonusAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal DeductionAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal GrossSalary { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal NetSalary { get; set; }

    public SchoolManagementSystem.Models.Enums.PayrollPaymentStatus PaymentStatus { get; set; } = SchoolManagementSystem.Models.Enums.PayrollPaymentStatus.Pending;
    public DateTime? PaymentDate { get; set; }

    [MaxLength(500)]
    public string? Remarks { get; set; }

    public int? GeneratedById { get; set; }
    [ForeignKey("GeneratedById")]
    public virtual SchoolManagementSystem.Models.Entities.Auth.ApplicationUser? GeneratedBy { get; set; }
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

    public int? ApprovedById { get; set; }
    [ForeignKey("ApprovedById")]
    public virtual SchoolManagementSystem.Models.Entities.Auth.ApplicationUser? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
}

public class Holiday
{
    [Key]
    public long Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public bool IsRecurring { get; set; }
}

public class EmployeeDocument
{
    [Key]
    public long Id { get; set; }

    [Required]
    public long EmployeeId { get; set; }

    [ForeignKey("EmployeeId")]
    public virtual Employee Employee { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string DocumentType { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string FilePath { get; set; } = string.Empty;

    [MaxLength(150)]
    public string? OriginalFileName { get; set; }

    [Required]
    public int UploadedById { get; set; }
    
    [ForeignKey("UploadedById")]
    public virtual SchoolManagementSystem.Models.Entities.Auth.ApplicationUser UploadedBy { get; set; } = null!;

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}
