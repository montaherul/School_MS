using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SchoolManagementSystem.Models.DTOs.Attendance;

namespace SchoolManagementSystem.Services.Interfaces.Attendance
{
    /// <summary>
    /// Interface for student attendance operations - aligned with EmployeeAttendanceService patterns
    /// </summary>
    public interface IStudentAttendanceService
    {
        // Individual record operations
        Task<int> MarkAttendanceAsync(StudentAttendanceItemDto dto, int classId, int sectionId, DateTime date, string recordedBy, CancellationToken ct = default);
        Task UpdateAttendanceAsync(int id, SchoolManagementSystem.Models.Enums.AttendanceStatus status, string? remarks, string updatedBy, CancellationToken ct = default);
        Task DeleteAttendanceAsync(int id, string deletedBy, CancellationToken ct = default);

        // Bulk operations
        Task<bool> BulkMarkAsync(StudentAttendanceBulkDto dto, string recordedBy, CancellationToken ct = default);
        Task<BulkAttendanceSaveResponse> SaveAttendanceAsync(StudentAttendanceBulkDto dto, string recordedBy, CancellationToken ct = default);

        // Data retrieval - list operations matching EmployeeAttendanceService
        Task<(List<StudentAttendanceDto> Data, int TotalRecords)> GetPagedAsync(
            int page, 
            int size, 
            int? classId = null, 
            int? sectionId = null, 
            int? studentGroupId = null,
            DateTime? date = null, 
            CancellationToken ct = default);

        Task<(List<StudentAttendanceDto> Data, int TotalRecords, StudentAttendanceSummaryDto Summary)> LoadAttendanceAsync(
            StudentAttendanceFilterDto filter,
            int page,
            int size,
            CancellationToken ct = default);

        // History and summary operations
        Task<List<StudentAttendanceDto>> GetAttendanceHistoryAsync(int studentId, int year, int month, CancellationToken ct = default);
        Task<StudentAttendanceMonthlySummaryDto> GetMonthlySummaryAsync(int studentId, int year, int month, CancellationToken ct = default);
        Task<double> GetAttendancePercentageAsync(int studentId, int year, int month, CancellationToken ct = default);

        // AJAX support for dynamic loading
        Task<(List<StudentAttendanceDto> Students, int Total)> GetStudentsForAttendanceAsync(
            int classId,
            int sectionId,
            int? studentGroupId,
            DateTime attendanceDate,
            int page = 1,
            int pageSize = 50,
            CancellationToken ct = default);

        // Session workflow operations
        Task UnlockAttendanceSessionAsync(int classId, int sectionId, int? studentGroupId, DateTime attendanceDate, string unlockedBy, string reason, CancellationToken ct = default);
        Task ReviseAttendanceSessionAsync(int classId, int sectionId, int? studentGroupId, DateTime attendanceDate, string revisedBy, string? notes, CancellationToken ct = default);
        Task ApproveAttendanceSessionAsync(int classId, int sectionId, int? studentGroupId, DateTime attendanceDate, string approvedBy, CancellationToken ct = default);
    }
}
