using SchoolManagementSystem.Models.DTOs.Accounting;

namespace SchoolManagementSystem.Models.ViewModels.Accounting;

public class ChartOfAccountViewModel : AccountUpsertDto
{
    public bool IsEditMode => Id > 0;
}
