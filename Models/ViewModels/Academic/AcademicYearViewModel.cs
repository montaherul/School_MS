using SchoolManagementSystem.Models.DTOs.Academic;

namespace SchoolManagementSystem.Models.ViewModels.Academic;

public class AcademicYearViewModel : AcademicYearUpsertDto
{
    public bool IsEditMode => Id > 0;
}
