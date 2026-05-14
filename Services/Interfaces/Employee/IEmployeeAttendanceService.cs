using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.DTOs.Employee;

namespace SchoolManagementSystem.Services.Interfaces.Employee;

public interface IEmployeeAttendanceService
{
    Task<IEnumerable<EmployeeAttendanceDto>> GetDailyAttendanceAsync(DateTime date, long? departmentId = null, CancellationToken ct = default);
    Task MarkAttendanceAsync(IEnumerable<EmployeeAttendanceDto> attendanceList, string createdBy, CancellationToken ct = default);
    Task<PagedResult<EmployeeAttendanceDto>> GetEmployeeHistoryPagedAsync(long employeeId, int page, int pageSize, DateTime? startDate = null, DateTime? endDate = null, CancellationToken ct = default);
    Task<EmployeeAttendanceSummaryDto> GetEmployeeSummaryAsync(long employeeId, DateTime? month = null, CancellationToken ct = default);
    Task<EmployeeAttendanceSummaryDto> GetDashboardSummaryAsync(DateTime date, CancellationToken ct = default);
    
    Task<PagedResult<EmployeeAttendanceDto>> GetPagedAsync(
        int page, int pageSize, string? search, long? departmentId, int? status, 
        DateTime? fromDate, DateTime? toDate, CancellationToken ct = default);
}
