using SchoolManagementSystem.Models.DTOs.Fees;

namespace SchoolManagementSystem.Models.ViewModels.Fees;

public class FeeInvoiceViewModel : FeeInvoiceUpsertDto
{
    public bool IsEditMode => Id > 0;
}
