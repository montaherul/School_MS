using SchoolManagementSystem.Models.Entities.Auth;
using SchoolManagementSystem.Models.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagementSystem.Models.Entities.Employee;

public class Employee : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string EmployeeCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(120)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(120)]
    public string? FatherName { get; set; }

    [MaxLength(120)]
    public string? MotherName { get; set; }

    [Required]
    [MaxLength(20)]
    public string Gender { get; set; } = string.Empty;

    public DateTime DateOfBirth { get; set; }

    [MaxLength(10)]
    public string? BloodGroup { get; set; }

    [MaxLength(50)]
    public string? Religion { get; set; }

    [MaxLength(50)]
    public string Nationality { get; set; } = "Bangladeshi";

    [MaxLength(50)]
    public string? NIDNumber { get; set; }

    [MaxLength(50)]
    public string? BirthCertificateNo { get; set; }

    [Required]
    [Phone]
    [MaxLength(30)]
    public string Phone { get; set; } = string.Empty;

    [EmailAddress]
    [MaxLength(160)]
    public string? Email { get; set; }

    [MaxLength(500)]
    public string? PresentAddress { get; set; }

    [MaxLength(500)]
    public string? PermanentAddress { get; set; }

    public DateTime JoiningDate { get; set; }

    public int DepartmentId { get; set; }
    public Department? Department { get; set; }

    public int DesignationId { get; set; }
    public Designation? Designation { get; set; }

    [MaxLength(50)]
    public string EmployeeType { get; set; } = "Full-Time";

    public bool IsTeachingStaff { get; set; }

    [MaxLength(20)]
    public string Status { get; set; } = "Active";

    public int? UserId { get; set; }
    public ApplicationUser? User { get; set; }

    [MaxLength(260)]
    public string? ProfilePicturePath { get; set; }

    [MaxLength(260)]
    public string? SignaturePath { get; set; }

    [MaxLength(120)]
    public string? EmergencyContactName { get; set; }

    [MaxLength(30)]
    public string? EmergencyContactPhone { get; set; }

    [MaxLength(500)]
    public string? Remarks { get; set; }

    // Navigations
    public ICollection<EmployeeQualification> Qualifications { get; set; } = new List<EmployeeQualification>();
    public ICollection<EmployeeDocument> Documents { get; set; } = new List<EmployeeDocument>();
    public ICollection<EmployeeExperience> Experiences { get; set; } = new List<EmployeeExperience>();
    public ICollection<SchoolManagementSystem.Models.Entities.Attendance.EmployeeAttendance> Attendances { get; set; } = new List<SchoolManagementSystem.Models.Entities.Attendance.EmployeeAttendance>();
    public ICollection<SchoolManagementSystem.Models.Entities.Attendance.LeaveApplication> Leaves { get; set; } = new List<SchoolManagementSystem.Models.Entities.Attendance.LeaveApplication>();
    public ICollection<EmployeeSalary> Salaries { get; set; } = new List<EmployeeSalary>();
}

public class Department : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
}

public class Designation : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public int RoleLevel { get; set; } = 0;

    public bool IsTeachingRole { get; set; } = false;
    
    public bool IsAdministrativeRole { get; set; } = false;
    
    public bool RequiresLogin { get; set; } = true;

    public bool IsActive { get; set; } = true;
}

public class DesignationRoleMapping : BaseEntity
{
    public int DesignationId { get; set; }
    public Designation? Designation { get; set; }

    public int RoleId { get; set; }
    public Role? Role { get; set; }

    public bool IsActive { get; set; } = true;
}

public class EmployeeQualification : BaseEntity
{
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    [Required]
    [MaxLength(100)]
    public string ExamName { get; set; } = string.Empty;

    [MaxLength(150)]
    public string? BoardOrUniversity { get; set; }

    [MaxLength(150)]
    public string? InstituteName { get; set; }

    [MaxLength(100)]
    public string? GroupOrSubject { get; set; }

    [MaxLength(10)]
    public string? PassingYear { get; set; }

    [MaxLength(50)]
    public string? Result { get; set; }

    [MaxLength(50)]
    public string? CGPAOrDivision { get; set; }

    [MaxLength(260)]
    public string? CertificateFilePath { get; set; }
}

public class EmployeeDocument : BaseEntity
{
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    [Required]
    [MaxLength(50)]
    public string DocumentType { get; set; } = string.Empty;

    [MaxLength(150)]
    public string DocumentName { get; set; } = string.Empty;

    [Required]
    [MaxLength(260)]
    public string FilePath { get; set; } = string.Empty;

    public DateTime? ExpiryDate { get; set; }

    [MaxLength(255)]
    public string? Remarks { get; set; }
}

public class EmployeeExperience : BaseEntity
{
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    [Required]
    [MaxLength(150)]
    public string OrganizationName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Designation { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    [MaxLength(500)]
    public string? Remarks { get; set; }
}


public class EmployeeSalary : BaseEntity
{
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    public decimal BasicSalary { get; set; }
    public decimal HouseRent { get; set; }
    public decimal MedicalAllowance { get; set; }
    public decimal TransportAllowance { get; set; }
    public decimal OtherAllowance { get; set; }
    public decimal Deduction { get; set; }
    public decimal TotalSalary { get; set; }

    public DateTime EffectiveFrom { get; set; }
}

public class EmployeeAcademicAssignment : BaseEntity
{
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    public int ClassId { get; set; }
    public int SectionId { get; set; }
    public int SubjectId { get; set; }
    public int AcademicYearId { get; set; }
}
