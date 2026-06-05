using SchoolManagementSystem.Models.Entities.Attendance;

namespace SchoolManagementSystem.Services.Interfaces.Attendance;

public interface IAttendanceNotificationService
{
    Task SendAbsentNotificationAsync(int studentId, DateOnly attendanceDate, string createdBy, CancellationToken ct = default);
    Task SendAbsentNotificationsAsync(IEnumerable<int> studentIds, DateOnly attendanceDate, string createdBy, CancellationToken ct = default);
    Task SendLateStudentNotificationsAsync(IEnumerable<int> studentIds, DateOnly attendanceDate, string createdBy, CancellationToken ct = default);
    Task SendLateEmployeeNotificationsAsync(IEnumerable<int> employeeIds, DateOnly attendanceDate, string createdBy, CancellationToken ct = default);
    Task<IReadOnlyList<AttendanceNotificationLog>> GetLogsAsync(DateOnly attendanceDate, int? classId = null, int? sectionId = null, CancellationToken ct = default);
}
