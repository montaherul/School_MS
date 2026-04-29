using SchoolManagementSystem.Models.DTOs.Academic;

namespace SchoolManagementSystem.Models.ViewModels.Academic;

public class SchoolClassViewModel : SchoolClassUpsertDto
{
    public bool IsEditMode => Id > 0;
}

