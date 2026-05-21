using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SchoolManagementSystem.Models.DTOs.Attendance;

namespace SchoolManagementSystem.Services.Interfaces.Attendance
{
    public interface IEmployeeAttendanceService
    {
        Task<int> CheckInAsync(int employeeId, DateTime date, TimeSpan time, string recordedBy, CancellationToken ct = default);
        Task<int> CheckOutAsync(int employeeId, DateTime date, TimeSpan time, string recordedBy, CancellationToken ct = default);
        Task<int> MarkStatusAsync(int employeeId, DateTime date, SchoolManagementSystem.Models.Enums.AttendanceStatus status, string? remarks, string recordedBy, CancellationToken ct = default);
        Task UpdateAttendanceAsync(int id, SchoolManagementSystem.Models.Enums.AttendanceStatus status, TimeSpan? checkIn, TimeSpan? checkOut, string? remarks, string updatedBy, CancellationToken ct = default);
        Task DeleteAttendanceAsync(int id, string deletedBy, CancellationToken ct = default);
        Task<bool> BulkMarkAsync(EmployeeAttendanceBulkDto dto, string recordedBy, CancellationToken ct = default);
        Task<bool> SaveAttendanceAsync(EmployeeAttendanceBulkDto dto, string recordedBy, CancellationToken ct = default);
        
        // Tabulator list integration
        Task<(List<EmployeeAttendanceDto> Data, int TotalRecords)> GetPagedAsync(int page, int size, DateTime? date, CancellationToken ct = default);
        Task<(List<EmployeeAttendanceDto> Data, int TotalRecords, EmployeeAttendanceSummaryDto Summary)> LoadAttendanceAsync(
            EmployeeAttendanceFilterDto filter,
            int page,
            int size,
            CancellationToken ct = default);
        Task<List<EmployeeAttendanceDto>> GetAttendanceHistoryAsync(int employeeId, int year, int month, CancellationToken ct = default);
        Task<EmployeeAttendanceMonthlySummaryDto> GetMonthlySummaryAsync(int employeeId, int year, int month, CancellationToken ct = default);
        
        Task<double> GetAttendancePercentageAsync(int employeeId, int year, int month, CancellationToken ct = default);
    }
}
