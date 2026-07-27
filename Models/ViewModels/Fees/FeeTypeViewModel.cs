using SchoolManagementSystem.Models.DTOs.Fees;

namespace SchoolManagementSystem.Models.ViewModels.Fees;

public class FeeTypeViewModel : FeeTypeUpsertDto
{
    public bool IsEditMode => Id > 0;
}
