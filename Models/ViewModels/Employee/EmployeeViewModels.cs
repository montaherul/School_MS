using SchoolManagementSystem.Models.DTOs.Employee;

namespace SchoolManagementSystem.Models.ViewModels.Employee;

public class EmployeeListItemViewModel : EmployeeListItemDto
{
    public string PhotoPath => ProfilePicturePath ?? string.Empty;
    public string Role => Designation;
    public string SchoolAddress => "[School Address]";
    public string SchoolPhone => "[School Phone]";
    public string SchoolEmail => "[School Email]";
    public string EmergencyContact => $"{EmergencyContactName ?? ""} {EmergencyContactPhone ?? ""}".Trim();
    public string NationalId => NIDNumber ?? string.Empty;
    public DateTime? ValidUntil => CardExpiryDate;
}

public class EmployeeDetailsViewModel : EmployeeDetailsDto
{
    public string PhotoPath => ProfilePicturePath ?? string.Empty;
    public string Role => Designation;
    public string SchoolAddress => "[School Address]";
    public string SchoolPhone => "[School Phone]";
    public string SchoolEmail => "[School Email]";
    public string EmergencyContact => $"{EmergencyContactName ?? ""} {EmergencyContactPhone ?? ""}".Trim();
    public string NationalId => NIDNumber ?? string.Empty;
    public DateTime? ValidUntil => CardExpiryDate;
}
