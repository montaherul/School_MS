using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagementSystem.Models.DTOs.Identity;

public class StudentIdCardListDto
{
    public int Id { get; set; }
    public string StudentCode { get; set; } = string.Empty;
    public int RollNumber { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string? PhotoPath { get; set; }
    public string? ClassName { get; set; }
    public string? SectionName { get; set; }
    public string? GroupName { get; set; }
    public string? Gender { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string Status { get; set; } = "Active";
    public string? GuardianName { get; set; }
    public DateTime? AdmissionDate { get; set; }

    [NotMapped]
    public int TotalRecords { get; set; }
}

public class EmployeeIdCardListDto
{
    public int Id { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public string? PhotoPath { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string Status { get; set; } = "Active";
    public bool IsTeachingStaff { get; set; }
    public string? EmploymentType { get; set; }
    public DateTime? JoiningDate { get; set; }
    public string? EmployeeCardNumber { get; set; }
    public DateTime? CardIssueDate { get; set; }
    public DateTime? CardExpiryDate { get; set; }
    public DateTime? CardPrintedAt { get; set; }
    public int CardVersion { get; set; }
    public string? DepartmentName { get; set; }
    public string? DesignationName { get; set; }

    [NotMapped]
    public int TotalRecords { get; set; }
}

public class StudentIdCardBulkDto
{
    public int Id { get; set; }
    public string? StudentNo { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? FullNameBangla { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string FatherName { get; set; } = string.Empty;
    public string? FatherOccupation { get; set; }
    public string MotherName { get; set; } = string.Empty;
    public string? MotherOccupation { get; set; }
    public string? GuardianName { get; set; }
    public string? GuardianMobileNumber { get; set; }
    public string MobileNumber { get; set; } = string.Empty;
    public string? EmailAddress { get; set; }
    public string? BloodGroup { get; set; }
    public string? Religion { get; set; }
    public int ClassId { get; set; }
    public int SectionId { get; set; }
    public int? StudentGroupId { get; set; }
    public int RollNumber { get; set; }
    public string? PresentVillage { get; set; }
    public string? PresentPostOffice { get; set; }
    public string? PresentThana { get; set; }
    public string? PresentDistrict { get; set; }
    public string? ProfilePicturePath { get; set; }
    public string? ClassName { get; set; }
    public string? SectionName { get; set; }
    public string? GroupName { get; set; }
}

public class EmployeeIdCardBulkDto
{
    public int Id { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? FatherName { get; set; }
    public string? MotherName { get; set; }
    public string Gender { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string? BloodGroup { get; set; }
    public string? NIDNumber { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? PresentAddress { get; set; }
    public DateTime JoiningDate { get; set; }
    public string Department { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public string EmployeeType { get; set; } = "Full-Time";
    public string Status { get; set; } = "Active";
    public string? ProfilePicturePath { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public string? EmployeeCardNumber { get; set; }
    public DateTime? CardIssueDate { get; set; }
    public DateTime? CardExpiryDate { get; set; }
    public int CardVersion { get; set; }
    public string? QRVerificationCode { get; set; }
}
