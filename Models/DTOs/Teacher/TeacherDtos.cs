using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace SchoolManagementSystem.Models.DTOs.Teacher;

// ── List (read-only, used in table/pagination) ────────────────────────────────
public class TeacherListItemDto
{
    public int Id { get; set; }
    public string TeacherNo { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? FullNameBangla { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string MobileNumber { get; set; } = string.Empty;
    public string? EmailAddress { get; set; }
    public string Designation { get; set; } = string.Empty;
    public string? Department { get; set; }
    public string? Qualification { get; set; }
    public string? Specialization { get; set; }
    public DateTime? JoiningDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? BloodGroup { get; set; }
    public string? Religion { get; set; }
    public string? Nationality { get; set; }
    public string? NationalIdNo { get; set; }
    public string? NationalIdPath { get; set; }
    public string? PassportNo { get; set; }
    public string? PassportPath { get; set; }
    public string? FatherName { get; set; }
    public string? MotherName { get; set; }
    public string? SpouseName { get; set; }
    public string? PresentVillage { get; set; }
    public string? PresentPostOffice { get; set; }
    public string? PresentThana { get; set; }
    public string? PresentDistrict { get; set; }
    public string? PermanentVillage { get; set; }
    public string? PermanentPostOffice { get; set; }
    public string? PermanentThana { get; set; }
    public string? PermanentDistrict { get; set; }
    public string? ProfilePicturePath { get; set; }
}

// ── Create / Edit (includes validation + file upload) ────────────────────────
public class TeacherUpsertDto
{
    public int Id { get; set; }

    public string TeacherNo { get; set; } = string.Empty;

    [Required, MaxLength(120), Display(Name = "Full Name (English)")]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(120), Display(Name = "Full Name (Bangla)")]
    public string? FullNameBangla { get; set; }

    [Required, Display(Name = "Date of Birth")]
    public DateTime DateOfBirth { get; set; } = DateTime.Today.AddYears(-25);

    [Required, MaxLength(20)]
    public string Gender { get; set; } = string.Empty;

    // ── Contact ──────────────────────────────────────────────────────────────
    [Required, MaxLength(30), Display(Name = "Mobile Number")]
    public string MobileNumber { get; set; } = string.Empty;

    [MaxLength(30), Display(Name = "Alternative Number")]
    public string? AlternativeNumber { get; set; }

    [EmailAddress, MaxLength(160), Display(Name = "Email Address")]
    public string? EmailAddress { get; set; }

    // ── Demographics ─────────────────────────────────────────────────────────
    [Required, MaxLength(50)]
    public string Nationality { get; set; } = "Bangladeshi";

    [Required, MaxLength(50)]
    public string Country { get; set; } = "Bangladesh";

    [MaxLength(30), Display(Name = "Marital Status")]
    public string MaritalStatus { get; set; } = string.Empty;

    [MaxLength(30)]
    public string Religion { get; set; } = string.Empty;

    [MaxLength(10), Display(Name = "Blood Group")]
    public string? BloodGroup { get; set; }

    // ── Identity ─────────────────────────────────────────────────────────────
    [MaxLength(50), Display(Name = "Passport No")]
    public string? PassportNo { get; set; }

    public string? PassportPath { get; set; }
    public IFormFile? PassportFile { get; set; }

    [MaxLength(50), Display(Name = "National ID No")]
    public string? NationalIdNo { get; set; }

    public string? NationalIdPath { get; set; }
    public IFormFile? NationalIdFile { get; set; }

    // ── Professional ─────────────────────────────────────────────────────────
    [Required, MaxLength(100)]
    public string Designation { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Department { get; set; }

    [MaxLength(200)]
    public string? Qualification { get; set; }

    [MaxLength(200)]
    public string? Specialization { get; set; }

    [Display(Name = "Joining Date")]
    public DateTime? JoiningDate { get; set; }

    // ── Family ───────────────────────────────────────────────────────────────
    [MaxLength(120), Display(Name = "Father's Name")]
    public string? FatherName { get; set; }

    [MaxLength(120), Display(Name = "Mother's Name")]
    public string? MotherName { get; set; }

    [MaxLength(120), Display(Name = "Spouse Name")]
    public string? SpouseName { get; set; }

    // ── Address ───────────────────────────────────────────────────────────────
    public string? PresentVillage { get; set; }

    [MaxLength(150), Display(Name = "Present Post Office")]
    public string? PresentPostOffice { get; set; }

    [MaxLength(150), Display(Name = "Present Thana")]
    public string? PresentThana { get; set; }

    [MaxLength(100), Display(Name = "Present District")]
    public string? PresentDistrict { get; set; }

    public string? PermanentVillage { get; set; }

    [MaxLength(150), Display(Name = "Permanent Post Office")]
    public string? PermanentPostOffice { get; set; }

    [MaxLength(150), Display(Name = "Permanent Thana")]
    public string? PermanentThana { get; set; }

    [MaxLength(100), Display(Name = "Permanent District")]
    public string? PermanentDistrict { get; set; }

    // ── Media ─────────────────────────────────────────────────────────────────
    public string? ProfilePicturePath { get; set; }
    public IFormFile? ProfilePicture { get; set; }

    // ── Workforce Status (read-only, derived from Employee) ────────────────────
    public string Status { get; set; } = "Active";
}