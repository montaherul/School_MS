using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.DTOs.Student;

namespace SchoolManagementSystem.Models.ViewModels.Student;

public class StudentIndexViewModel
{
    public List<SchoolClassListItemDto> Classes { get; set; } = new();
}

public class IdCardPrintViewModel
{
    public string BaseUrl { get; set; } = string.Empty;
    public List<StudentUpsertDto> Students { get; set; } = [];
    public bool IsBulk { get; set; }

    public string SchoolLogoPath { get; set; } = string.Empty;
    public string SchoolSealPath { get; set; } = string.Empty;
    public string SchoolNameEn { get; set; } = string.Empty;
    public string SchoolNameBn { get; set; } = string.Empty;
    public string SchoolEIIN { get; set; } = string.Empty;
    public string SchoolWebsite { get; set; } = string.Empty;
    public string SchoolMotto { get; set; } = string.Empty;
    public string AcademicYear { get; set; } = string.Empty;
    public string SchoolAddress { get; set; } = string.Empty;
    public string SchoolPhone { get; set; } = string.Empty;
    public string SchoolEmail { get; set; } = string.Empty;
    public string PrincipalName { get; set; } = string.Empty;
    public string PrincipalSignaturePath { get; set; } = string.Empty;
    public string? FooterText { get; set; }
}

public class StudentCardModel
{
    public StudentUpsertDto Student { get; set; } = null!;
    public string QrCodeDataUri { get; set; } = string.Empty;

    public string SchoolLogoPath { get; set; } = string.Empty;
    public string SchoolSealPath { get; set; } = string.Empty;
    public string SchoolNameEn { get; set; } = string.Empty;
    public string SchoolNameBn { get; set; } = string.Empty;
    public string SchoolEIIN { get; set; } = string.Empty;
    public string SchoolWebsite { get; set; } = string.Empty;
    public string SchoolMotto { get; set; } = string.Empty;
    public string AcademicYear { get; set; } = string.Empty;
    public string SchoolAddress { get; set; } = string.Empty;
    public string SchoolPhone { get; set; } = string.Empty;
    public string SchoolEmail { get; set; } = string.Empty;
    public string PrincipalName { get; set; } = string.Empty;
    public string PrincipalSignaturePath { get; set; } = string.Empty;
    public string? FooterText { get; set; }
}
