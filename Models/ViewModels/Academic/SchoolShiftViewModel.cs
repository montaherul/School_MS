using SchoolManagementSystem.Models.DTOs.Academic;

namespace SchoolManagementSystem.Models.ViewModels.Academic;

public class SchoolShiftViewModel : SchoolShiftUpsertDto
{
    public bool IsEditMode => Id > 0;
}
