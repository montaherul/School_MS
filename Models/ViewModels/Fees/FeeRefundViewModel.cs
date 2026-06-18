using SchoolManagementSystem.Models.DTOs.Fees;

namespace SchoolManagementSystem.Models.ViewModels.Fees;

public class FeeRefundViewModel : FeeRefundUpsertDto
{
    public bool IsEditMode => Id > 0;
}
