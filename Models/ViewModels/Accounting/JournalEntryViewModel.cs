using SchoolManagementSystem.Models.DTOs.Accounting;

namespace SchoolManagementSystem.Models.ViewModels.Accounting;

public class JournalEntryViewModel : JournalEntryUpsertDto
{
    public bool IsEditMode => Id > 0;
}
