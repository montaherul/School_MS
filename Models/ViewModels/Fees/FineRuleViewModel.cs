using SchoolManagementSystem.Models.DTOs.Fees;

namespace SchoolManagementSystem.Models.ViewModels.Fees;

public class FineRuleViewModel : FineRuleUpsertDto
{
    public bool IsEditMode => Id > 0;
}
