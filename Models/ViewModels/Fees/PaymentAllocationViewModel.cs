using SchoolManagementSystem.Models.DTOs.Fees;

namespace SchoolManagementSystem.Models.ViewModels.Fees;

public class PaymentAllocationViewModel : PaymentAllocationUpsertDto
{
    public bool IsEditMode => Id > 0;
}
