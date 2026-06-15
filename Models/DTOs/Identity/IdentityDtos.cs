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
