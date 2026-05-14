using SchoolManagementSystem.Models.Entities.Employee;
using SchoolManagementSystem.Repositories.Interfaces;

namespace SchoolManagementSystem.Repositories.Interfaces.Employee;

public interface IEmployeeLeaveRepository : IBaseRepository<EmployeeLeave>
{
    Task<IEnumerable<EmployeeLeave>> GetEmployeeLeaveHistoryAsync(long employeeId, DateTime? startDate = null, DateTime? endDate = null, CancellationToken ct = default);
    Task<bool> HasOverlapAsync(long employeeId, DateTime startDate, DateTime endDate, long? excludeLeaveId = null, CancellationToken ct = default);
}

public interface ILeaveTypeRepository : IBaseRepository<LeaveType>
{
}
