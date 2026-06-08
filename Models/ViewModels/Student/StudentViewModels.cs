using SchoolManagementSystem.Models.DTOs.Academic;

namespace SchoolManagementSystem.Models.ViewModels.Student;

public class StudentIndexViewModel
{
    public List<SchoolClassListItemDto> Classes { get; set; } = new();
}
