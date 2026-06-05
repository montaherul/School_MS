using SchoolManagementSystem.Models.Entities.Attendance;
using SchoolManagementSystem.Models.DTOs.Attendance;

namespace SchoolManagementSystem.Repositories.Interfaces.Attendance;

public interface IAttendanceRepository : IBaseRepository<AttendanceRecord>
{
    Task<(List<AttendanceRecordListItemDto> items, int totalRecords)> GetListByStoredProcedureAsync(
        int pageNumber, int pageSize, string? searchTerm,
        int studentId, int classId, int sectionId, int studentGroupId, int status,
        DateOnly? attendanceDate, string? sortColumn, string? sortDirection,
        CancellationToken ct);
}
