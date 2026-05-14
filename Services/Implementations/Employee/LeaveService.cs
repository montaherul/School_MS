using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.DTOs.Employee;
using SchoolManagementSystem.Models.Entities.Employee;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.Employee;
using SchoolManagementSystem.Repositories.Interfaces.Auth;
using SchoolManagementSystem.Services.Interfaces.Employee;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Employee;

public class EmployeeLeaveService : IEmployeeLeaveService
{
    private readonly IUnitOfWork _uow;
    private readonly IEmployeeLeaveRepository _leaveRepo;
    private readonly ILeaveTypeRepository _leaveTypeRepo;
    private readonly IEmployeeAttendanceRepository _attendanceRepo;
    private readonly IAuditLogRepository _auditLogRepo;

    public EmployeeLeaveService(
        IUnitOfWork uow,
        IEmployeeLeaveRepository leaveRepo,
        ILeaveTypeRepository leaveTypeRepo,
        IEmployeeAttendanceRepository attendanceRepo,
        IAuditLogRepository auditLogRepo)
    {
        _uow = uow;
        _leaveRepo = leaveRepo;
        _leaveTypeRepo = leaveTypeRepo;
        _attendanceRepo = attendanceRepo;
        _auditLogRepo = auditLogRepo;
    }

    public async Task<long> ApplyLeaveAsync(EmployeeLeaveDto dto, string appliedBy, CancellationToken ct = default)
    {
        if (dto.StartDate.Date < DateTime.Today.Date)
            throw new InvalidOperationException("Leave cannot be applied for past dates.");

        if (dto.EndDate.Date < dto.StartDate.Date)
            throw new InvalidOperationException("End date cannot be before start date.");

        if (await _leaveRepo.HasOverlapAsync(dto.EmployeeId, dto.StartDate, dto.EndDate, null, ct))
            throw new InvalidOperationException("This employee already has a leave application during this period.");

        int totalDays = (dto.EndDate.Date - dto.StartDate.Date).Days + 1;

        var leave = new EmployeeLeave
        {
            EmployeeId = dto.EmployeeId,
            LeaveTypeId = dto.LeaveTypeId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            TotalDays = totalDays,
            Reason = dto.Reason,
            Status = LeaveStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _leaveRepo.AddAsync(leave, ct);
        
        await _auditLogRepo.AddAsync(new SchoolManagementSystem.Models.Entities.Auth.AuditLog
        {
            Module = "EmployeeLeave",
            Action = "Apply",
            Details = $"Leave applied for EmployeeId: {dto.EmployeeId}, Dates: {dto.StartDate:yyyy-MM-dd} to {dto.EndDate:yyyy-MM-dd}",
            CreatedBy = appliedBy,
            CreatedAt = DateTime.UtcNow
        }, ct);

        await _uow.SaveChangesAsync(ct);
        return leave.Id;
    }

    public async Task ApproveLeaveAsync(long leaveId, string remarks, long approvedByUserId, CancellationToken ct = default)
    {
        var leave = await _leaveRepo.Query()
            .Include(l => l.Employee)
            .FirstOrDefaultAsync(l => l.Id == leaveId, ct)
            ?? throw new KeyNotFoundException("Leave application not found.");

        if (leave.Status != LeaveStatus.Pending)
            throw new InvalidOperationException("Only pending leave requests can be approved.");

        leave.Status = LeaveStatus.Approved;
        leave.ApprovedById = approvedByUserId;
        leave.ApprovedAt = DateTime.UtcNow;
        leave.Remarks = remarks;
        leave.UpdatedAt = DateTime.UtcNow;

        _leaveRepo.Update(leave);

        // Sync with Attendance
        for (var date = leave.StartDate.Date; date <= leave.EndDate.Date; date = date.AddDays(1))
        {
            var attendance = await _attendanceRepo.FirstOrDefaultAsync(a => a.EmployeeId == leave.EmployeeId && a.AttendanceDate.Date == date, ct);
            if (attendance == null)
            {
                await _attendanceRepo.AddAsync(new EmployeeAttendance
                {
                    EmployeeId = leave.EmployeeId,
                    AttendanceDate = date,
                    Status = AttendanceStatus.Leave,
                    Remarks = $"Approved Leave: {leave.Reason}",
                    CreatedBy = "System-LeaveSync",
                    CreatedAt = DateTime.UtcNow
                }, ct);
            }
            else
            {
                attendance.Status = AttendanceStatus.Leave;
                attendance.Remarks = $"Updated by Leave Approval: {leave.Reason}";
                attendance.UpdatedAt = DateTime.UtcNow;
                attendance.UpdatedBy = "System-LeaveSync";
                _attendanceRepo.Update(attendance);
            }
        }

        await _auditLogRepo.AddAsync(new SchoolManagementSystem.Models.Entities.Auth.AuditLog
        {
            Module = "EmployeeLeave",
            Action = "Approve",
            Details = $"Leave approved for LeaveId: {leaveId}, Employee: {leave.Employee.FullName}",
            CreatedBy = approvedByUserId.ToString(),
            CreatedAt = DateTime.UtcNow
        }, ct);

        await _uow.SaveChangesAsync(ct);
    }

    public async Task RejectLeaveAsync(long leaveId, string reason, long rejectedByUserId, CancellationToken ct = default)
    {
        var leave = await _leaveRepo.FirstOrDefaultAsync(l => l.Id == leaveId, ct)
            ?? throw new KeyNotFoundException("Leave application not found.");

        if (leave.Status != LeaveStatus.Pending)
            throw new InvalidOperationException("Only pending leave requests can be rejected.");

        leave.Status = LeaveStatus.Rejected;
        leave.RejectionReason = reason;
        leave.UpdatedAt = DateTime.UtcNow;

        _leaveRepo.Update(leave);

        await _auditLogRepo.AddAsync(new SchoolManagementSystem.Models.Entities.Auth.AuditLog
        {
            Module = "EmployeeLeave",
            Action = "Reject",
            Details = $"Leave rejected for LeaveId: {leaveId}, Reason: {reason}",
            CreatedBy = rejectedByUserId.ToString(),
            CreatedAt = DateTime.UtcNow
        }, ct);

        await _uow.SaveChangesAsync(ct);
    }

    public async Task CancelLeaveAsync(long leaveId, string cancelledBy, CancellationToken ct = default)
    {
        var leave = await _leaveRepo.FirstOrDefaultAsync(l => l.Id == leaveId, ct)
            ?? throw new KeyNotFoundException("Leave application not found.");

        if (leave.Status == LeaveStatus.Cancelled || leave.Status == LeaveStatus.Rejected)
            throw new InvalidOperationException("Leave is already cancelled or rejected.");

        var oldStatus = leave.Status;
        leave.Status = LeaveStatus.Cancelled;
        leave.UpdatedAt = DateTime.UtcNow;

        _leaveRepo.Update(leave);

        // If it was approved, we should ideally revert attendance, but usually attendance is marked 'Absent' or 'Pending' if leave is cancelled.
        // For simplicity, we'll just remove/revert the 'Leave' status in attendance if it exists for those dates.
        if (oldStatus == LeaveStatus.Approved)
        {
            var attendanceRecords = await _attendanceRepo.Query()
                .Where(a => a.EmployeeId == leave.EmployeeId && a.AttendanceDate >= leave.StartDate && a.AttendanceDate <= leave.EndDate && a.Status == AttendanceStatus.Leave)
                .ToListAsync(ct);

            foreach (var att in attendanceRecords)
            {
                // Revert to absent or delete? Usually, we might want to delete if it was auto-created.
                _attendanceRepo.Remove(att);
            }
        }

        await _auditLogRepo.AddAsync(new SchoolManagementSystem.Models.Entities.Auth.AuditLog
        {
            Module = "EmployeeLeave",
            Action = "Cancel",
            Details = $"Leave cancelled for LeaveId: {leaveId}",
            CreatedBy = cancelledBy,
            CreatedAt = DateTime.UtcNow
        }, ct);

        await _uow.SaveChangesAsync(ct);
    }

    public async Task<PagedResult<EmployeeLeaveDto>> GetPagedAsync(int page, int pageSize, string? search, long? departmentId = null, long? leaveTypeId = null, LeaveStatus? status = null, CancellationToken ct = default)
    {
        var query = _leaveRepo.Query()
            .Include(l => l.Employee).ThenInclude(e => e.Department)
            .Include(l => l.LeaveType)
            .Include(l => l.ApprovedBy)
            .AsNoTracking();

        if (!string.IsNullOrEmpty(search))
            query = query.Where(l => l.Employee.FullName.Contains(search) || l.Employee.EmployeeCode.Contains(search));

        if (departmentId.HasValue) query = query.Where(l => l.Employee.DepartmentId == departmentId.Value);
        if (leaveTypeId.HasValue) query = query.Where(l => l.LeaveTypeId == leaveTypeId.Value);
        if (status.HasValue) query = query.Where(l => l.Status == status.Value);

        var totalItems = await query.CountAsync(ct);
        var items = await query.OrderByDescending(l => l.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(l => new EmployeeLeaveDto
            {
                Id = l.Id,
                EmployeeId = l.EmployeeId,
                EmployeeName = l.Employee.FullName,
                EmployeeCode = l.Employee.EmployeeCode,
                LeaveTypeId = l.LeaveTypeId,
                LeaveTypeName = l.LeaveType.Name,
                StartDate = l.StartDate,
                EndDate = l.EndDate,
                TotalDays = l.TotalDays,
                Reason = l.Reason,
                Status = l.Status,
                ApprovedByName = l.ApprovedBy != null ? l.ApprovedBy.UserName : null,
                ApprovedAt = l.ApprovedAt,
                RejectionReason = l.RejectionReason,
                Remarks = l.Remarks,
                CreatedAt = l.CreatedAt
            })
            .ToListAsync(ct);

        return new PagedResult<EmployeeLeaveDto> { Items = items, TotalItems = totalItems, Page = page, PageSize = pageSize };
    }

    public async Task<EmployeeLeaveDto?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        return await _leaveRepo.Query()
            .Include(l => l.Employee).ThenInclude(e => e.Department)
            .Include(l => l.LeaveType)
            .Include(l => l.ApprovedBy)
            .Where(l => l.Id == id)
            .Select(l => new EmployeeLeaveDto
            {
                Id = l.Id,
                EmployeeId = l.EmployeeId,
                EmployeeName = l.Employee.FullName,
                EmployeeCode = l.Employee.EmployeeCode,
                LeaveTypeId = l.LeaveTypeId,
                LeaveTypeName = l.LeaveType.Name,
                StartDate = l.StartDate,
                EndDate = l.EndDate,
                TotalDays = l.TotalDays,
                Reason = l.Reason,
                Status = l.Status,
                ApprovedByName = l.ApprovedBy != null ? l.ApprovedBy.UserName : null,
                ApprovedAt = l.ApprovedAt,
                RejectionReason = l.RejectionReason,
                Remarks = l.Remarks,
                CreatedAt = l.CreatedAt
            })
            .FirstOrDefaultAsync(ct);
    }

    public async Task<EmployeeLeaveSummaryDto> GetEmployeeLeaveSummaryAsync(long employeeId, int year, CancellationToken ct = default)
    {
        var leaveTypes = await _leaveTypeRepo.Query().Where(t => t.IsActive).ToListAsync(ct);
        var approvedLeaves = await _leaveRepo.Query()
            .Where(l => l.EmployeeId == employeeId && l.Status == LeaveStatus.Approved && l.StartDate.Year == year)
            .ToListAsync(ct);

        var summary = new EmployeeLeaveSummaryDto
        {
            ApprovedLeaves = approvedLeaves.Count,
            PendingRequests = await _leaveRepo.CountAsync(l => l.EmployeeId == employeeId && l.Status == LeaveStatus.Pending, ct),
            RejectedLeaves = await _leaveRepo.CountAsync(l => l.EmployeeId == employeeId && l.Status == LeaveStatus.Rejected, ct),
            TotalLeaveTaken = approvedLeaves.Sum(l => l.TotalDays)
        };

        foreach (var type in leaveTypes)
        {
            summary.Balances.Add(new LeaveBalanceDto
            {
                LeaveTypeName = type.Name,
                Allowed = type.DefaultDaysPerYear,
                Taken = approvedLeaves.Where(l => l.LeaveTypeId == type.Id).Sum(l => l.TotalDays)
            });
        }

        summary.RemainingBalance = summary.Balances.Sum(b => b.Remaining);

        return summary;
    }
}

public class LeaveTypeService : ILeaveTypeService
{
    private readonly ILeaveTypeRepository _repo;

    public LeaveTypeService(ILeaveTypeRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<LeaveTypeDto>> GetAllAsync(bool onlyActive = true, CancellationToken ct = default)
    {
        var query = _repo.Query();
        if (onlyActive) query = query.Where(t => t.IsActive);

        return await query.Select(t => new LeaveTypeDto
        {
            Id = t.Id,
            Name = t.Name,
            DefaultDaysPerYear = t.DefaultDaysPerYear,
            IsPaid = t.IsPaid,
            ColorCode = t.ColorCode,
            IsActive = t.IsActive
        }).ToListAsync(ct);
    }
}
