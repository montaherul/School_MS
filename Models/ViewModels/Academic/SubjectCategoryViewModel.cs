using SchoolManagementSystem.Models.DTOs.Academic;

namespace SchoolManagementSystem.Models.ViewModels.Academic;

public class SubjectCategoryViewModel : SubjectCategoryUpsertDto
{
    public bool IsEditMode => Id > 0;
}
