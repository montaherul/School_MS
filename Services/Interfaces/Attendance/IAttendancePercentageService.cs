using System.Threading;
using System.Threading.Tasks;
using SchoolManagementSystem.Models.DTOs.Attendance;

namespace SchoolManagementSystem.Services.Interfaces.Attendance
{
    public interface IAttendancePercentageService
    {
        Task<double> GetStudentAttendancePercentageAsync(int studentId, int year, int month, CancellationToken ct = default);
        Task<StudentAttendanceStatsDto> GetStudentAttendanceStatsAsync(int studentId, int year, int month, CancellationToken ct = default);
        Task<double> GetEmployeeAttendancePercentageAsync(int employeeId, int year, int month, CancellationToken ct = default);
        Task<EmployeeAttendanceStatsDto> GetEmployeeAttendanceStatsAsync(int employeeId, int year, int month, CancellationToken ct = default);
    }
}
