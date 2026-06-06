using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using SchoolManagementSystem.Models.DTOs.Attendance;
using SchoolManagementSystem.Models.Entities.Attendance;

namespace SchoolManagementSystem.Repositories.Interfaces.Attendance
{
    public interface IStudentAttendanceRepository : IBaseRepository<StudentAttendance>
    {
        Task<bool> IsAttendanceExistsAsync(int studentId, System.DateTime date, CancellationToken cancellationToken = default);
        Task<(List<StudentAttendanceDto> Items, int TotalRecords)> GetAttendanceGridAsync(
            StudentAttendanceFilterDto filter,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default);
        Task<StudentAttendanceSummaryDto> GetAttendanceSummaryAsync(
            StudentAttendanceFilterDto filter,
            CancellationToken cancellationToken = default);
        Task<List<StudentAttendanceDto>> GetStudentHistoryAsync(
            int studentId,
            int year,
            int month,
            CancellationToken cancellationToken = default);
    }

    public interface IEmployeeAttendanceRepository : IBaseRepository<EmployeeAttendance>
    {
        Task<bool> IsAttendanceExistsAsync(int employeeId, System.DateTime date, CancellationToken cancellationToken = default);
        Task<(List<EmployeeAttendanceDto> Items, int TotalRecords)> GetAttendanceGridAsync(
            EmployeeAttendanceFilterDto filter,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default);
        Task<EmployeeAttendanceSummaryDto> GetAttendanceSummaryAsync(
            EmployeeAttendanceFilterDto filter,
            CancellationToken cancellationToken = default);
        Task<List<EmployeeAttendanceDto>> GetEmployeeHistoryAsync(
            int employeeId,
            int year,
            int month,
            CancellationToken cancellationToken = default);
    }

    public interface ILeaveTypeRepository : IBaseRepository<LeaveType>
    {
    }

    public interface ILeaveApplicationRepository : IBaseRepository<LeaveApplication>
    {
        Task<bool> HasOverlappingLeaveAsync( int employeeId,DateTime fromDate,DateTime toDate,CancellationToken ct = default);
    }

    public interface IAttendanceSettingRepository : IBaseRepository<AttendanceSetting>
    {
        Task<AttendanceSetting?> GetCurrentSettingsAsync(CancellationToken cancellationToken = default);
    }

    public interface IAttendanceLogRepository : IBaseRepository<AttendanceLog>
    {
    }
}
