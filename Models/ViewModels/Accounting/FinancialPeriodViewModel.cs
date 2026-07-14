using SchoolManagementSystem.Models.DTOs.Accounting;

namespace SchoolManagementSystem.Models.ViewModels.Accounting;

public class FinancialPeriodViewModel : FinancialPeriodUpsertDto
{
    public bool IsEditMode => Id > 0;
}
