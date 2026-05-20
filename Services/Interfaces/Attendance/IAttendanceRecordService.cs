using SchoolManagementSystem.Models.DTOs.Attendance;
using SchoolManagementSystem.Models.DTOs.Common;

namespace SchoolManagementSystem.Services.Interfaces.Attendance;

public interface IAttendanceRecordService
{
    Task<PagedResult<AttendanceRecordListItemDto>> GetPagedAsync(
        int page, int pageSize, string? search,
        int? studentId = null,
        int? classId   = null,
        int? sectionId = null,
        DateOnly? attendanceDate = null,
        CancellationToken cancellationToken = default);




    Task<AttendanceRecordUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default);
    Task<int>  CreateAsync(AttendanceRecordUpsertDto dto, string createdBy, CancellationToken cancellationToken = default);
    Task       UpdateAsync(AttendanceRecordUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default);
    Task       DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default);
}

