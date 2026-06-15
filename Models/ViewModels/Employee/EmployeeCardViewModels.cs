using SchoolManagementSystem.Models.DTOs.Employee;

namespace SchoolManagementSystem.Models.ViewModels.Employee;

public class EmployeeIdCardPrintViewModel
{
    public string BaseUrl { get; set; } = string.Empty;
    public List<EmployeeDetailsDto> Employees { get; set; } = [];
    public bool IsBulk => Employees.Count > 1;

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


