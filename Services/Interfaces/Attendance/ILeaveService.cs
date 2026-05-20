using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SchoolManagementSystem.Models.DTOs.Attendance;
using SchoolManagementSystem.Models.ViewModels.Attendance;

namespace SchoolManagementSystem.Services.Interfaces.Attendance
{
    public interface ILeaveService
    {
        Task<int> ApplyLeaveAsync(LeaveApplyVm vm, int employeeId, string attachmentPath, CancellationToken ct = default);
        Task ApproveLeaveAsync(int id, string approvedBy, string? remarks, CancellationToken ct = default);
        Task RejectLeaveAsync(int id, string rejectedBy, string? remarks, CancellationToken ct = default);
        Task CancelLeaveAsync(int id, int employeeId, CancellationToken ct = default);

        Task<(List<LeaveApplicationDto> Data, int TotalRecords)> GetMyLeavesAsync(int employeeId, int page, int size, CancellationToken ct = default);
        Task<(List<LeaveApplicationDto> Data, int TotalRecords)> GetPendingLeavesAsync(int page, int size, CancellationToken ct = default);
        Task<(List<LeaveApplicationDto> Data, int TotalRecords)> GetAllLeavesAsync(int page, int size, string? status, CancellationToken ct = default);
        
        Task<List<LeaveTypeDto>> GetActiveLeaveTypesAsync(CancellationToken ct = default);
    }
}
