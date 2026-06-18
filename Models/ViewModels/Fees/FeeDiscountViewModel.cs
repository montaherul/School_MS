using SchoolManagementSystem.Models.DTOs.Fees;

namespace SchoolManagementSystem.Models.ViewModels.Fees;

public class FeeDiscountViewModel : FeeDiscountUpsertDto
{
    public bool IsEditMode => Id > 0;
}
