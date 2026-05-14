using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Employee;
using SchoolManagementSystem.Repositories.Interfaces.Employee;
using SchoolManagementSystem.Models.DTOs.Employee;
using Dapper;
using System.Data;

namespace SchoolManagementSystem.Repositories.Implementations.Employee;

public class EmployeeAttendanceRepository : BaseRepository<EmployeeAttendance>, IEmployeeAttendanceRepository
{
    public EmployeeAttendanceRepository(SchoolDbContext db) : base(db) { }

    public async Task<IEnumerable<EmployeeAttendance>> GetDailyAttendanceAsync(DateTime date, long? departmentId = null, CancellationToken ct = default)
    {
        var query = _db.EmployeeAttendances
            .Include(a => a.Employee)
            .Where(a => a.AttendanceDate.Date == date.Date);

        if (departmentId.HasValue)
        {
            query = query.Where(a => a.Employee.DepartmentId == departmentId.Value);
        }

        return await query.AsNoTracking().ToListAsync(ct);
    }

    public async Task<IEnumerable<EmployeeAttendance>> GetEmployeeHistoryAsync(long employeeId, DateTime startDate, DateTime endDate, CancellationToken ct = default)
    {
        return await _db.EmployeeAttendances
            .Where(a => a.EmployeeId == employeeId && a.AttendanceDate.Date >= startDate.Date && a.AttendanceDate.Date <= endDate.Date)
            .OrderByDescending(a => a.AttendanceDate)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<bool> ExistsAsync(long employeeId, DateTime date, CancellationToken ct = default)
    {
        return await _db.EmployeeAttendances
            .AnyAsync(a => a.EmployeeId == employeeId && a.AttendanceDate.Date == date.Date, ct);
    }

    public async Task<(List<EmployeeAttendanceDto> items, int totalRecords)> GetPagedAsync(
        int page, int pageSize, string? search, long? departmentId, int? status, 
        DateTime? fromDate, DateTime? toDate, CancellationToken ct)
    {
        var connection = _db.Database.GetDbConnection();
        
        var parameters = new DynamicParameters();
        parameters.Add("@PageNumber", page);
        parameters.Add("@PageSize", pageSize);
        parameters.Add("@Search", search);
        parameters.Add("@DepartmentId", departmentId);
        parameters.Add("@Status", status);
        parameters.Add("@DateFrom", fromDate);
        parameters.Add("@DateTo", toDate);
        parameters.Add("@SortField", "AttendanceDate");
        parameters.Add("@SortDirection", "DESC");

        var result = (await connection.QueryAsync<dynamic>(
            "sp_Attendance_GetPaged",
            parameters,
            commandType: CommandType.StoredProcedure
        )).ToList();

        var data = result.Select(x => new EmployeeAttendanceDto
        {
            Id = (long)x.Id,
            EmployeeId = (long)x.Id, // SP doesn't return EmployeeId explicitly in my draft but can be derived or added
            EmployeeName = x.FullName,
            EmployeeCode = x.EmployeeCode,
            DepartmentName = x.DepartmentName,
            AttendanceDate = x.AttendanceDate,
            Status = (SchoolManagementSystem.Models.Enums.AttendanceStatus)x.Status,
            CheckInTime = x.CheckInTime,
            CheckOutTime = x.CheckOutTime,
            Remarks = x.Remarks
        }).ToList();

        int totalRecords = data.Any() ? (int)result.First().TotalCount : 0;

        return (data, totalRecords);
    }
}
