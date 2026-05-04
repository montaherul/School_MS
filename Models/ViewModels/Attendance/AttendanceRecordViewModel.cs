using SchoolManagementSystem.Models.DTOs.Attendance;

namespace SchoolManagementSystem.Models.ViewModels.Attendance;

public class AttendanceRecordViewModel : AttendanceRecordUpsertDto
{
    public bool IsEditMode => Id > 0;
}

