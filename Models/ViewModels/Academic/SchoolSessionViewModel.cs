using SchoolManagementSystem.Models.DTOs.Academic;

namespace SchoolManagementSystem.Models.ViewModels.Academic;

public class SchoolSessionViewModel : SchoolSessionUpsertDto
{
    public bool IsEditMode => Id > 0;
}
