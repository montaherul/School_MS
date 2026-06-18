using SchoolManagementSystem.Models.DTOs.Fees;

namespace SchoolManagementSystem.Models.ViewModels.Fees;

public class StudentFeeAssignmentViewModel : StudentFeeAssignmentUpsertDto
{
    public bool IsEditMode => Id > 0;
}
