using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.DTOs.Teacher;

namespace SchoolManagementSystem.Services.Interfaces.Teachers;

public interface ITeacherHRService
{
    // Attendance
    Task<IEnumerable<TeacherAttendanceDto>> GetAttendanceAsync(DateTime date, string? department = null);
    Task MarkAttendanceAsync(IEnumerable<TeacherAttendanceDto> attendanceList, string userId);
    Task<PagedResult<TeacherAttendanceDto>> GetTeacherAttendanceHistoryAsync(int teacherId, int page, int size);

    // Leave
    Task<PagedResult<TeacherLeaveDto>> GetLeavesPagedAsync(int page, int size, string? status = null);
    Task RequestLeaveAsync(TeacherLeaveDto dto, string userId);
    Task ApproveLeaveAsync(int leaveId, string remarks, string userId);
    Task RejectLeaveAsync(int leaveId, string remarks, string userId);

    // Payroll
    Task<PagedResult<TeacherPayrollDto>> GetPayrollPagedAsync(int page, int size, DateTime? monthYear = null);
    Task GenerateMonthlyPayrollAsync(DateTime monthYear, string userId);
    Task UpdatePayrollStatusAsync(int payrollId, string status, string userId);
}
