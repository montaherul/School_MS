using SchoolManagementSystem.Models.DTOs.Fees;

namespace SchoolManagementSystem.Models.ViewModels.Fees;

public class FeePaymentViewModel : FeePaymentUpsertDto
{
    public bool IsEditMode => Id > 0;
}
