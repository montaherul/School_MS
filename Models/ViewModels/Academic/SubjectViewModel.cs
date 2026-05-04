using SchoolManagementSystem.Models.DTOs.Academic;

namespace SchoolManagementSystem.Models.ViewModels.Academic;

public class SubjectViewModel : SubjectUpsertDto
{
    public bool IsEditMode => Id > 0;
}

