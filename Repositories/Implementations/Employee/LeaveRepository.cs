using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Employee;
using SchoolManagementSystem.Repositories.Interfaces.Employee;

namespace SchoolManagementSystem.Repositories.Implementations.Employee;

public class EmployeeLeaveRepository : BaseRepository<EmployeeLeave>, IEmployeeLeaveRepository
{
    public EmployeeLeaveRepository(SchoolDbContext db) : base(db) { }

    public async Task<IEnumerable<EmployeeLeave>> GetEmployeeLeaveHistoryAsync(long employeeId, DateTime? startDate = null, DateTime? endDate = null, CancellationToken ct = default)
    {
        var query = _db.EmployeeLeaves
            .Include(l => l.LeaveType)
            .Include(l => l.ApprovedBy)
            .Where(l => l.EmployeeId == employeeId);

        if (startDate.HasValue) query = query.Where(l => l.StartDate >= startDate.Value);
        if (endDate.HasValue) query = query.Where(l => l.EndDate <= endDate.Value);

        return await query.OrderByDescending(l => l.StartDate).AsNoTracking().ToListAsync(ct);
    }

    public async Task<bool> HasOverlapAsync(long employeeId, DateTime startDate, DateTime endDate, long? excludeLeaveId = null, CancellationToken ct = default)
    {
        return await _db.EmployeeLeaves
            .AnyAsync(l => l.EmployeeId == employeeId 
                        && l.Id != (excludeLeaveId ?? 0)
                        && l.Status != SchoolManagementSystem.Models.Enums.LeaveStatus.Rejected
                        && l.Status != SchoolManagementSystem.Models.Enums.LeaveStatus.Cancelled
                        && l.StartDate <= endDate 
                        && l.EndDate >= startDate, ct);
    }
}

public class LeaveTypeRepository : BaseRepository<LeaveType>, ILeaveTypeRepository
{
    public LeaveTypeRepository(SchoolDbContext db) : base(db) { }
}
