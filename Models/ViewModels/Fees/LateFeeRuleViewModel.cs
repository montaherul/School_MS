using SchoolManagementSystem.Models.DTOs.Fees;

namespace SchoolManagementSystem.Models.ViewModels.Fees;

public class LateFeeRuleViewModel : LateFeeRuleUpsertDto
{
    public bool IsEditMode => Id > 0;
}
