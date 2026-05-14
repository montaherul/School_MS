using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Employee;
using SchoolManagementSystem.Repositories.Interfaces.Employee;

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
}
