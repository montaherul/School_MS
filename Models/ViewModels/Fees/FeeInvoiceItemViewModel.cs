using SchoolManagementSystem.Models.DTOs.Fees;

namespace SchoolManagementSystem.Models.ViewModels.Fees;

public class FeeInvoiceItemViewModel : FeeInvoiceItemUpsertDto
{
    public bool IsEditMode => Id > 0;
}
