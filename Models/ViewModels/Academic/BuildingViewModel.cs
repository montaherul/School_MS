using SchoolManagementSystem.Models.DTOs.Academic;

namespace SchoolManagementSystem.Models.ViewModels.Academic;

public class BuildingViewModel : BuildingUpsertDto
{
    public bool IsEditMode => Id > 0;
}
