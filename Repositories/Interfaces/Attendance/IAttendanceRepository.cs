using SchoolManagementSystem.Models.Entities.Attendance;
using SchoolManagementSystem.Models.DTOs.Attendance;

namespace SchoolManagementSystem.Repositories.Interfaces.Attendance;

public interface IAttendanceRepository : IBaseRepository<AttendanceRecord>
{
    Task<(List<AttendanceRecordListItemDto> items, int totalRecords)> GetListByStoredProcedureAsync(
<<<<<<< HEAD
        int pageNumber, int pageSize, string? searchTerm, int studentId, CancellationToken ct);
=======
        int pageNumber, int pageSize, string? searchTerm,
        int studentId, int classId, int sectionId, DateOnly? attendanceDate,
        CancellationToken ct);
>>>>>>> d8b24e6 (attendece and website curtomize)
}
