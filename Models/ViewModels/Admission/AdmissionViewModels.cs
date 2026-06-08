using SchoolManagementSystem.Models.DTOs.Admission;

namespace SchoolManagementSystem.Models.ViewModels.Admission;

public class AdmissionListViewModel : AdmissionListResultDto
{
    public int Age => DateTime.Today.Year - DateOfBirth.Year;
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
