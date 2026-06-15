using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.DTOs.Employee;

namespace SchoolManagementSystem.Models.ViewModels.IdCard;

public class IdCardStudentListViewModel
{
    public List<SchoolClassListItemDto> Classes { get; set; } = [];
    public List<SectionListItemDto> Groups { get; set; } = [];
}

public class IdCardEmployeeListViewModel
{
    public List<DepartmentDto> Departments { get; set; } = [];
    public List<DesignationDto> Designations { get; set; } = [];
}
