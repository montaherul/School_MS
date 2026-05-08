namespace SchoolManagementSystem.Models.DTOs.Admission.StoredProcedures;

/// <summary>
/// DTO for Admission List results from sp_GetAdmissionList stored procedure
/// </summary>
public class AdmissionListResultDto
{
    public int Id { get; set; }
    public string ApplicationNo { get; set; }
    public string ApplicantName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; }
    public int AppliedClassId { get; set; }
    public string ClassName { get; set; }
    public string ApplicantMobileNumber { get; set; }
    public string FatherOrGuardianMobileNo { get; set; }
    public string AlternativeNumber { get; set; }
    public string ApplicantEmail { get; set; }
    public string Status { get; set; }
    public string FatherName { get; set; }
    public string FatherOccupation { get; set; }
    public string MotherName { get; set; }
    public string MotherOccupation { get; set; }
    public string GuardianName { get; set; }
    public string GuardianOccupation { get; set; }
    public string Nationality { get; set; }
    public string Religion { get; set; }
    public string BloodGroup { get; set; }
    public string NationalIdNo { get; set; }
    public string BirthCertificateNo { get; set; }
    public string PassportNo { get; set; }
    public string PaymentMethod { get; set; }
    public string TransactionDetails { get; set; }
    public string PresentVillage { get; set; }
    public string PresentPostOffice { get; set; }
    public string PresentThana { get; set; }
    public string PresentDistrict { get; set; }
    public string PermanentVillage { get; set; }
    public string PermanentPostOffice { get; set; }
    public string PermanentThana { get; set; }
    public string PermanentDistrict { get; set; }
    public string ProfilePicturePath { get; set; }
    public int TotalRecords { get; set; }

    // Computed properties for UI
    public int Age => DateTime.Today.Year - DateOfBirth.Year;
    public string CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedAtFormatted => CreatedAt.ToString("dd-MMM-yyyy");
    public int DaysApplied => (DateTime.Today - CreatedAt).Days;

    public string StatusBadgeClass => Status switch
    {
        "Approved" => "badge-success",
        "Rejected" => "badge-danger",
        "Under Review" => "badge-warning",
        _ => "badge-secondary"
    };
}
