using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.DTOs.Employee;
using SchoolManagementSystem.Models.Entities.Employee;
using SchoolManagementSystem.Models.Entities.Auth;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.Employee;
using SchoolManagementSystem.Repositories.Interfaces.Auth;
using SchoolManagementSystem.Services.Interfaces.Employee;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Employee;

public class EmployeeAttendanceService : IEmployeeAttendanceService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmployeeAttendanceRepository _attendanceRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IAuditLogRepository _auditLogRepository;

    public EmployeeAttendanceService(
        IUnitOfWork unitOfWork,
        IEmployeeAttendanceRepository attendanceRepository,
        IEmployeeRepository employeeRepository,
        IAuditLogRepository auditLogRepository)
    {
        _unitOfWork = unitOfWork;
        _attendanceRepository = attendanceRepository;
        _employeeRepository = employeeRepository;
        _auditLogRepository = auditLogRepository;
    }

    public async Task<IEnumerable<EmployeeAttendanceDto>> GetDailyAttendanceAsync(DateTime date, long? departmentId = null, CancellationToken ct = default)
    {
        var activeEmployees = await _employeeRepository.Query()
            .Include(e => e.Department)
            .Where(e => e.IsActive)
            .Where(e => !departmentId.HasValue || e.DepartmentId == departmentId.Value)
            .ToListAsync(ct);

        var attendanceRecords = await _attendanceRepository.GetDailyAttendanceAsync(date, departmentId, ct);
        var attendanceMap = attendanceRecords.ToDictionary(a => a.EmployeeId);

        return activeEmployees.Select(e => new EmployeeAttendanceDto
        {
            EmployeeId = e.Id,
            EmployeeName = e.FullName,
            EmployeeCode = e.EmployeeCode,
            DepartmentName = e.Department.Name,
            AttendanceDate = date,
            Id = attendanceMap.TryGetValue(e.Id, out var att) ? att.Id : 0,
            Status = attendanceMap.TryGetValue(e.Id, out var att2) ? att2.Status : AttendanceStatus.Present,
            CheckInTime = attendanceMap.TryGetValue(e.Id, out var att3) ? att3.CheckInTime : null,
            CheckOutTime = attendanceMap.TryGetValue(e.Id, out var att4) ? att4.CheckOutTime : null,
            Remarks = attendanceMap.TryGetValue(e.Id, out var att5) ? att5.Remarks : null
        });
    }

    public async Task MarkAttendanceAsync(IEnumerable<EmployeeAttendanceDto> attendanceList, string createdBy, CancellationToken ct = default)
    {
        foreach (var dto in attendanceList)
        {
            var existing = await _attendanceRepository.FirstOrDefaultAsync(a => a.EmployeeId == dto.EmployeeId && a.AttendanceDate.Date == dto.AttendanceDate.Date, ct);
            
            if (existing == null)
            {
                var newAttendance = new EmployeeAttendance
                {
                    EmployeeId = dto.EmployeeId,
                    AttendanceDate = dto.AttendanceDate,
                    Status = dto.Status,
                    CheckInTime = dto.CheckInTime,
                    CheckOutTime = dto.CheckOutTime,
                    Remarks = dto.Remarks,
                    CreatedBy = createdBy,
                    CreatedAt = DateTime.UtcNow
                };
                await _attendanceRepository.AddAsync(newAttendance, ct);
            }
            else
            {
                existing.Status = dto.Status;
                existing.CheckInTime = dto.CheckInTime;
                existing.CheckOutTime = dto.CheckOutTime;
                existing.Remarks = dto.Remarks;
                existing.UpdatedBy = createdBy;
                existing.UpdatedAt = DateTime.UtcNow;
                _attendanceRepository.Update(existing);
            }
        }

        await _auditLogRepository.AddAsync(new AuditLog
        {
            Module = "EmployeeAttendance",
            Action = "BulkMark",
            Details = $"Bulk attendance marked for {attendanceList.Count()} employees for date {attendanceList.FirstOrDefault()?.AttendanceDate:yyyy-MM-dd}",
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        }, ct);

        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<PagedResult<EmployeeAttendanceDto>> GetEmployeeHistoryPagedAsync(long employeeId, int page, int pageSize, DateTime? startDate = null, DateTime? endDate = null, CancellationToken ct = default)
    {
        var query = _attendanceRepository.Query()
            .Where(a => a.EmployeeId == employeeId);

        if (startDate.HasValue) query = query.Where(a => a.AttendanceDate >= startDate.Value);
        if (endDate.HasValue) query = query.Where(a => a.AttendanceDate <= endDate.Value);

        var totalItems = await query.CountAsync(ct);
        var items = await query.OrderByDescending(a => a.AttendanceDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new EmployeeAttendanceDto
            {
                Id = a.Id,
                AttendanceDate = a.AttendanceDate,
                Status = a.Status,
                CheckInTime = a.CheckInTime,
                CheckOutTime = a.CheckOutTime,
                Remarks = a.Remarks
            })
            .ToListAsync(ct);

        return new PagedResult<EmployeeAttendanceDto> { Items = items, TotalItems = totalItems, Page = page, PageSize = pageSize };
    }

    public async Task<EmployeeAttendanceSummaryDto> GetEmployeeSummaryAsync(long employeeId, DateTime? month = null, CancellationToken ct = default)
    {
        var targetMonth = month ?? DateTime.Today;
        var startDate = new DateTime(targetMonth.Year, targetMonth.Month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        var records = await _attendanceRepository.Query()
            .Where(a => a.EmployeeId == employeeId && a.AttendanceDate >= startDate && a.AttendanceDate <= endDate)
            .ToListAsync(ct);

        int totalDaysInMonth = DateTime.DaysInMonth(targetMonth.Year, targetMonth.Month);
        // Simple logic: working days are all days for now (can be refined with holidays)
        
        var summary = new EmployeeAttendanceSummaryDto
        {
            TotalPresent = records.Count(r => r.Status == AttendanceStatus.Present),
            TotalAbsent = records.Count(r => r.Status == AttendanceStatus.Absent),
            TotalLate = records.Count(r => r.Status == AttendanceStatus.Late),
            TotalLeave = records.Count(r => r.Status == AttendanceStatus.Leave),
            LastAttendanceDate = records.OrderByDescending(r => r.AttendanceDate).FirstOrDefault()?.AttendanceDate
        };

        if (records.Any())
        {
            summary.AttendancePercentage = Math.Round((double)summary.TotalPresent / records.Count * 100, 2);
        }

        return summary;
    }

    public async Task<EmployeeAttendanceSummaryDto> GetDashboardSummaryAsync(DateTime date, CancellationToken ct = default)
    {
        var records = await _attendanceRepository.Query()
            .Where(a => a.AttendanceDate.Date == date.Date)
            .ToListAsync(ct);

        return new EmployeeAttendanceSummaryDto
        {
            TotalPresent = records.Count(r => r.Status == AttendanceStatus.Present),
            TotalAbsent = records.Count(r => r.Status == AttendanceStatus.Absent),
            TotalLate = records.Count(r => r.Status == AttendanceStatus.Late),
            TotalLeave = records.Count(r => r.Status == AttendanceStatus.Leave)
        };
    }
}
