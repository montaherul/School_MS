using SchoolManagementSystem.Models.DTOs.Academic;

namespace SchoolManagementSystem.Models.ViewModels.Academic;

public class SectionViewModel : SectionUpsertDto
{
    public bool IsEditMode => Id > 0;
}

