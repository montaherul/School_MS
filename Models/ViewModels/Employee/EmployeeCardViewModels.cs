using SchoolManagementSystem.Models.DTOs.Employee;

namespace SchoolManagementSystem.Models.ViewModels.Employee;

public class EmployeeIdCardPrintViewModel
{
    public List<EmployeeDetailsDto> Employees { get; set; } = [];
    public bool IsBulk => Employees.Count > 1;

    public string SchoolLogoPath { get; set; } = string.Empty;
    public string SchoolNameEn { get; set; } = string.Empty;
    public string SchoolEIIN { get; set; } = string.Empty;
    public string SchoolWebsite { get; set; } = string.Empty;
    public string SchoolAddress { get; set; } = string.Empty;
    public string SchoolPhone { get; set; } = string.Empty;
    public string SchoolEmail { get; set; } = string.Empty;
    public string PrincipalName { get; set; } = string.Empty;
    public string PrincipalSignaturePath { get; set; } = string.Empty;
}

public class EmployeeIdCardBulkFilterViewModel
{
    public List<DepartmentDto> Departments { get; set; } = [];
    public List<DesignationDto> Designations { get; set; } = [];
    public int? DepartmentId { get; set; }
    public int? DesignationId { get; set; }
    public string? Status { get; set; }
}
