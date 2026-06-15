using SchoolManagementSystem.Models.DTOs.Employee;

namespace SchoolManagementSystem.Models.ViewModels.Employee;

public class EmployeeIdCardPrintViewModel
{
    public string BaseUrl { get; set; } = string.Empty;
    public List<EmployeeDetailsDto> Employees { get; set; } = [];
    public bool IsBulk { get; set; }

    public string SchoolLogoPath { get; set; } = string.Empty;
    public string SchoolSealPath { get; set; } = string.Empty;
    public string SchoolNameEn { get; set; } = string.Empty;
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

public class EmployeeCardModel
{
    public EmployeeDetailsDto Employee { get; set; } = null!;
    public string QrCodeDataUri { get; set; } = string.Empty;
    public string ThemeColor { get; set; } = "#1B4D8C";

    public string SchoolLogoPath { get; set; } = string.Empty;
    public string SchoolSealPath { get; set; } = string.Empty;
    public string SchoolNameEn { get; set; } = string.Empty;
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
