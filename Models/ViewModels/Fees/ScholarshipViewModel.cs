using SchoolManagementSystem.Models.DTOs.Fees;

namespace SchoolManagementSystem.Models.ViewModels.Fees;

public class ScholarshipViewModel : ScholarshipUpsertDto
{
    public bool IsEditMode => Id > 0;
}
