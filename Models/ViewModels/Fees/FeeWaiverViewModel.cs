using SchoolManagementSystem.Models.DTOs.Fees;

namespace SchoolManagementSystem.Models.ViewModels.Fees;

public class FeeWaiverViewModel : FeeWaiverUpsertDto
{
    public bool IsEditMode => Id > 0;
}
