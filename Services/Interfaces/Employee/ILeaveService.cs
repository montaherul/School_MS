using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.DTOs.Employee;

namespace SchoolManagementSystem.Services.Interfaces.Employee;

public interface IEmployeeLeaveService
{
    Task<long> ApplyLeaveAsync(EmployeeLeaveDto dto, string appliedBy, CancellationToken ct = default);
    Task ApproveLeaveAsync(long leaveId, string remarks, long approvedByUserId, CancellationToken ct = default);
    Task RejectLeaveAsync(long leaveId, string reason, long rejectedByUserId, CancellationToken ct = default);
    Task CancelLeaveAsync(long leaveId, string cancelledBy, CancellationToken ct = default);
    
    Task<PagedResult<EmployeeLeaveDto>> GetPagedAsync(int page, int pageSize, string? search, long? departmentId = null, long? leaveTypeId = null, SchoolManagementSystem.Models.Enums.LeaveStatus? status = null, CancellationToken ct = default);
    Task<EmployeeLeaveDto?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<EmployeeLeaveSummaryDto> GetEmployeeLeaveSummaryAsync(long employeeId, int year, CancellationToken ct = default);
}

public interface ILeaveTypeService
{
    Task<IEnumerable<LeaveTypeDto>> GetAllAsync(bool onlyActive = true, CancellationToken ct = default);
}
