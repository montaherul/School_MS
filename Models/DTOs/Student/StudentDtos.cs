using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagementSystem.Models.DTOs.Student;

public class StudentListItemDto
{
    public int Id { get; set; }
    public string StudentNo { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? FullNameBangla { get; set; }
    public int ClassId { get; set; }
    public int SectionId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string SectionName { get; set; } = string.Empty;
    public int RollNumber { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? FatherName { get; set; }
    public string? FatherOccupation { get; set; }
    public string? MotherName { get; set; }
    public string? MotherOccupation { get; set; }

    public string? MobileNumber { get; set; }
    public string? EmailAddress { get; set; }

    public string? PresentVillage { get; set; }
    public string? PresentPostOffice { get; set; }
    public string? PresentThana { get; set; }
    public string? PresentDistrict { get; set; }

    public string? PermanentVillage { get; set; }
    public string? PermanentPostOffice { get; set; }
    public string? PermanentThana { get; set; }
    public string? PermanentDistrict { get; set; }

    public string? BloodGroup { get; set; }
    public string? Religion { get; set; }
    public string? Nationality { get; set; }

    public string? BirthCertificateNo { get; set; }

    public string? FatherOrGuardianMobileNo { get; set; }
    [NotMapped]
    public IFormFile? ProfilePicture { get; set; }
    public string? ProfilePicturePath { get; set; }
    public int TotalRecords { get; set; }
}

public class StudentUpsertDto
{
    public int Id { get; set; }
    public string? StudentNo { get; set; }

    [Required, MaxLength(120), Display(Name="Student's Name (English)")]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(120), Display(Name="Student's Name (Bangla)")]
    public string? FullNameBangla { get; set; }

    [Required, Display(Name="Date of Birth")]
    public DateTime DateOfBirth { get; set; } = DateTime.Today.AddYears(-6);

    [Required, MaxLength(20)]
    public string Gender { get; set; } = string.Empty;

    [Required, MaxLength(120), Display(Name="Father's Name")]
    public string FatherName { get; set; } = string.Empty;

    [MaxLength(100), Display(Name="Father's Occupation")]
    public string? FatherOccupation { get; set; }

    [Required, MaxLength(120), Display(Name="Mother's Name")]
    public string MotherName { get; set; } = string.Empty;

    [MaxLength(100), Display(Name="Mother's Occupation")]
    public string? MotherOccupation { get; set; }

    [MaxLength(120), Display(Name="Guardian's Name")]
    public string? GuardianName { get; set; }

    [MaxLength(100), Display(Name="Guardian's Occupation")]
    public string? GuardianOccupation { get; set; }

    public int? LinkedGuardianId { get; set; }

    [MaxLength(30), Display(Name="Father/Guardian Mobile No.")]
    public string FatherOrGuardianMobileNo { get; set; } = string.Empty;

    [Required, MaxLength(30), Display(Name="Student's Mobile Number")]
    public string MobileNumber { get; set; } = string.Empty;

    [MaxLength(30), Display(Name="Alternative Number")]
    public string? AlternativeNumber { get; set; }

    [EmailAddress, MaxLength(160), Display(Name="Student's Email")]
    public string? EmailAddress { get; set; }

    [Required, MaxLength(50)]
    public string Nationality { get; set; } = "Bangladeshi";

    [Required, MaxLength(50)]
    public string Country { get; set; } = "Bangladesh";

    [MaxLength(30), Display(Name="Marital Status")]
    public string MaritalStatus { get; set; } = string.Empty;

    [MaxLength(30)]
    public string Religion { get; set; } = string.Empty;

    [MaxLength(10), Display(Name="Blood Group")]
    public string? BloodGroup { get; set; }


    [MaxLength(50), Display(Name="Birth Certificate No.")]
    public string? BirthCertificateNo { get; set; }

    [Range(1, int.MaxValue)]
    public int ClassId { get; set; }

    [Range(1, int.MaxValue)]
    public int SectionId { get; set; }

    public int? StudentGroupId { get; set; }

    public int? OptionalSubjectId { get; set; }

    [Range(1, 500)]
    public int RollNumber { get; set; }
    // Present
    public string? PresentVillage { get; set; }
    [MaxLength(150)]
    public string? PresentPostOffice { get; set; }
    [MaxLength(150)]
    public string? PresentThana { get; set; }
    [MaxLength(150)]
    public string? PresentDistrict { get; set; }

    // Permanent
    public string? PermanentVillage { get; set; }
    [MaxLength(150)]
    public string? PermanentPostOffice { get; set; }
    [MaxLength(150)]
    public string? PermanentThana { get; set; }
    [MaxLength(150)]
    public string? PermanentDistrict { get; set; }
    // Navigation props mapped
    public string? ProfilePicturePath { get; set; }

    [NotMapped]
    public IFormFile? ProfilePicture { get; set; }
    public int? UserId { get; set; }
    public string? SectionName { get; set; }
    public string? ClassName { get; set; }
    public string? GroupName { get; set; }
    public List<SelectListItem> Sections { get; set; } = new();

    public List<SelectListItem> OptionalSubjectList { get; set; } = new();

    // Guardian extended fields (populated from StudentGuardian -> Guardian)
    public string? GuardianEmail { get; set; }
    public string? GuardianMobileNumber { get; set; }
    public string? GuardianRelationship { get; set; }
    public string? GuardianNationalId { get; set; }
    public string? GuardianAddress { get; set; }
    public string? GuardianPhoto { get; set; }
    public bool? GuardianUserCreated { get; set; }
    public string? GuardianCode { get; set; }
    public int LinkedChildrenCount { get; set; }
}

public class StudentClassSectionDto
{
    public int ClassId { get; set; }
    public int SectionId { get; set; }
}
