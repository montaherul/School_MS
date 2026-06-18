using SchoolManagementSystem.Models.DTOs.Fees;

namespace SchoolManagementSystem.Models.ViewModels.Fees;

public class FeeStructureViewModel : FeeStructureUpsertDto
{
    public bool IsEditMode => Id > 0;
}
