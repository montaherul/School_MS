using SchoolManagementSystem.Models.DTOs.Fees;

namespace SchoolManagementSystem.Models.ViewModels.Fees;

public class FeeCategoryViewModel : FeeCategoryUpsertDto
{
    public bool IsEditMode => Id > 0;
}
