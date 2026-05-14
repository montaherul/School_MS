using SchoolManagementSystem.Models.Entities.Employee;
using SchoolManagementSystem.Repositories.Interfaces;
using SchoolManagementSystem.Models.DTOs.Employee;

namespace SchoolManagementSystem.Repositories.Interfaces.Employee;

public interface IEmployeeAttendanceRepository : IBaseRepository<EmployeeAttendance>
{
    Task<IEnumerable<EmployeeAttendance>> GetDailyAttendanceAsync(DateTime date, long? departmentId = null, CancellationToken ct = default);
    Task<IEnumerable<EmployeeAttendance>> GetEmployeeHistoryAsync(long employeeId, DateTime startDate, DateTime endDate, CancellationToken ct = default);
    Task<bool> ExistsAsync(long employeeId, DateTime date, CancellationToken ct = default);
    Task<(List<EmployeeAttendanceDto> items, int totalRecords)> GetPagedAsync(
        int page, int pageSize, string? search, long? departmentId, int? status, 
        DateTime? fromDate, DateTime? toDate, CancellationToken ct);
}
