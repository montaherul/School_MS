using SchoolManagementSystem.Models.DTOs.Attendance;
using SchoolManagementSystem.Models.DTOs.Dashboard;
using SchoolManagementSystem.Models.DTOs.Student;

namespace SchoolManagementSystem.Services.Interfaces.Student;

public interface IStudentPortalService
{
    Task<StudentPortalDashboardDto> GetDashboardAsync(int userId, CancellationToken ct = default);
    Task<StudentProfileDto?> GetProfileAsync(int userId, CancellationToken ct = default);
    Task UpdateProfileAsync(int userId, StudentProfileUpdateDto dto, CancellationToken ct = default);
    Task<List<StudentAttendanceDto>> GetAttendanceAsync(int userId, DateTime? from, DateTime? to, CancellationToken ct = default);
    Task<List<StudentNotificationItemDto>> GetNotificationsAsync(int userId, CancellationToken ct = default);
    Task MarkNotificationReadAsync(int userId, int notificationId, CancellationToken ct = default);
}
