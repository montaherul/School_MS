namespace SchoolManagementSystem.Models.ViewModels.IdCard;

public class EmployeeIdCardViewModel
{
    public string EmployeeFullName { get; set; } = string.Empty;
    public string EmployeeIdNo { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string MobileNumber { get; set; } = string.Empty;
    public string BloodGroup { get; set; } = string.Empty;
    public string DateOfBirth { get; set; } = string.Empty;
    public string BadgeLabel { get; set; } = "Staff";
    public string SignatureLabel { get; set; } = "Chairman";

    public string EmployeePhotoDataUri { get; set; } = string.Empty;
    public string SchoolName { get; set; } = string.Empty;
    public string SchoolShortName { get; set; } = string.Empty;
    public string SchoolLogoDataUri { get; set; } = string.Empty;
    public string SchoolAddress { get; set; } = string.Empty;
    public string SchoolPhone { get; set; } = string.Empty;
    public string SchoolEmail { get; set; } = string.Empty;

    public string QrCodeDataUri { get; set; } = string.Empty;
    public string ValidUntil { get; set; } = string.Empty;
}
