using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SchoolManagementSystem.Models.DTOs.Student;

public class StudentProfileDto
{
    public int Id { get; set; }
    public string StudentNo { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? FullNameBangla { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string FatherName { get; set; } = string.Empty;
    public string MotherName { get; set; } = string.Empty;
    public string MobileNumber { get; set; } = string.Empty;
    public string? EmailAddress { get; set; }
    public string? ProfilePicturePath { get; set; }
    public string? PresentAddress { get; set; }
    public string? PermanentAddress { get; set; }
    public string? BloodGroup { get; set; }
    public string Religion { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string SectionName { get; set; } = string.Empty;
    public int RollNumber { get; set; }
    public string? StudentGroupName { get; set; }
}

public class StudentProfileUpdateDto
{
    public int Id { get; set; }

    [MaxLength(80)]
    public string? EmailAddress { get; set; }

    [MaxLength(30)]
    public string? MobileNumber { get; set; }

    [MaxLength(250)]
    public string? PresentVillage { get; set; }

    [MaxLength(250)]
    public string? PermanentVillage { get; set; }

    public string? CurrentPassword { get; set; }
    public string? NewPassword { get; set; }

    public IFormFile? PhotoFile { get; set; }
}

public class StudentUpsertDto
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
    public string MobileNumber { get; set; } = string.Empty;
    public string? AlternativeNumber { get; set; }
    public string? EmailAddress { get; set; }
    public string Nationality { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string? MaritalStatus { get; set; }
    public string Religion { get; set; } = string.Empty;
    public string? BloodGroup { get; set; }
    public string? PresentVillage { get; set; }
    public string? PresentPostOffice { get; set; }
    public string? PresentThana { get; set; }
    public string? PresentDistrict { get; set; }
    public string? PermanentVillage { get; set; }
    public string? PermanentPostOffice { get; set; }
    public string? PermanentThana { get; set; }
    public string? PermanentDistrict { get; set; }
    public string? BirthCertificateNo { get; set; }
    public string? ProfilePicturePath { get; set; }
    public int ClassId { get; set; }
    public int SectionId { get; set; }
    public int RollNumber { get; set; }
    public int? OptionalSubjectId { get; set; }
    public int? StudentGroupId { get; set; }
    public string? SectionName { get; set; }
    public string? ClassName { get; set; }
    public string? GroupName { get; set; }
    public string? GuardianName { get; set; }
    public string? GuardianOccupation { get; set; }
    public string? GuardianEmail { get; set; }
    public string? GuardianMobileNumber { get; set; }
    public string? GuardianRelationship { get; set; }
    public string? GuardianNationalId { get; set; }
    public string? GuardianAddress { get; set; }
    public string? GuardianPhoto { get; set; }
    public bool GuardianUserCreated { get; set; }
    public string? GuardianCode { get; set; }
    public int LinkedChildrenCount { get; set; }
    public string? FatherOrGuardianMobileNo { get; set; }
    public int? UserId { get; set; }
    public int? LinkedGuardianId { get; set; }
    public List<SelectListItem> Sections { get; set; } = new();
    public List<SelectListItem> OptionalSubjectList { get; set; } = new();
    public IFormFile? ProfilePicture { get; set; }
}

public class StudentListItemDto
{
    public int Id { get; set; }
    public string StudentNo { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? FullNameBangla { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string MobileNumber { get; set; } = string.Empty;
    public string? EmailAddress { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string SectionName { get; set; } = string.Empty;
    public int RollNumber { get; set; }
    public string? BloodGroup { get; set; }
    public string FatherName { get; set; } = string.Empty;
    public string MotherName { get; set; } = string.Empty;
    public string? GuardianName { get; set; }
    public string? ProfilePicturePath { get; set; }
    public bool IsActive { get; set; }
    public int TotalRecords { get; set; }
}

public class StudentClassSectionDto
{
    public int ClassId { get; set; }
    public int SectionId { get; set; }
}

public class StudentPortalDashboardDto
{
    public string StudentName { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string SectionName { get; set; } = string.Empty;
    public int RollNumber { get; set; }
    public string? ProfilePicturePath { get; set; }
    public double AttendancePercentage { get; set; }
    public int PresentCount { get; set; }
    public int AbsentCount { get; set; }
    public int LateCount { get; set; }
    public int LeaveCount { get; set; }
    public decimal OutstandingFees { get; set; }
    public decimal TotalPaid { get; set; }
    public int InvoiceCount { get; set; }
    public decimal? LatestGPA { get; set; }
    public string? LatestGrade { get; set; }
    public bool LatestPassed { get; set; }
    public int UnreadNotificationCount { get; set; }
    public int PendingLeaveCount { get; set; }
    public bool IsResultBlocked { get; set; }
    public decimal TotalInvoiced { get; set; }
    public decimal TotalDue { get; set; }
    public int LeaveApplicationCount { get; set; }
}
