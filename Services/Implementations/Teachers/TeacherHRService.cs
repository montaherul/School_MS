using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.DTOs.Teacher;
using SchoolManagementSystem.Models.Entities.Teachers;
using SchoolManagementSystem.Services.Interfaces.Teachers;

namespace SchoolManagementSystem.Services.Implementations.Teachers;

public class TeacherHRService : ITeacherHRService
{
    private readonly SchoolDbContext _context;

    public TeacherHRService(SchoolDbContext context)
    {
        _context = context;
    }

    // ── Attendance ───────────────────────────────────────────────────────────

    public async Task<IEnumerable<TeacherAttendanceDto>> GetAttendanceAsync(DateTime date, string? department = null)
    {
        var teachers = await _context.Teachers
            .Where(t => !t.IsDeleted && t.Status == SchoolManagementSystem.Models.Enums.TeacherStatus.Active)
            .Where(t => string.IsNullOrEmpty(department) || t.Department == department)
            .ToListAsync();

        var existingAttendance = await _context.TeacherAttendances
            .Where(a => a.AttendanceDate.Date == date.Date)
            .ToDictionaryAsync(a => a.TeacherId);

        return teachers.Select(t => new TeacherAttendanceDto
        {
            TeacherId = t.Id,
            TeacherName = t.FullName,
            TeacherNo = t.TeacherNo,
            AttendanceDate = date,
            Status = existingAttendance.ContainsKey(t.Id) ? existingAttendance[t.Id].Status : "Present",
            Remarks = existingAttendance.ContainsKey(t.Id) ? existingAttendance[t.Id].Remarks : string.Empty,
            Id = existingAttendance.ContainsKey(t.Id) ? existingAttendance[t.Id].Id : 0
        });
    }

    public async Task MarkAttendanceAsync(IEnumerable<TeacherAttendanceDto> attendanceList, string userId)
    {
        foreach (var dto in attendanceList)
        {
            var attendance = await _context.TeacherAttendances
                .FirstOrDefaultAsync(a => a.AttendanceDate.Date == dto.AttendanceDate.Date && a.TeacherId == dto.TeacherId);

            if (attendance == null)
            {
                attendance = new TeacherAttendance
                {
                    TeacherId = dto.TeacherId,
                    AttendanceDate = dto.AttendanceDate,
                    Status = dto.Status,
                    Remarks = dto.Remarks ?? string.Empty,
                    CreatedBy = userId,
                    CreatedAt = DateTime.UtcNow
                };
                _context.TeacherAttendances.Add(attendance);
            }
            else
            {
                attendance.Status = dto.Status;
                attendance.Remarks = dto.Remarks ?? string.Empty;
                attendance.UpdatedBy = userId;
                attendance.UpdatedAt = DateTime.UtcNow;
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task<PagedResult<TeacherAttendanceDto>> GetTeacherAttendanceHistoryAsync(int teacherId, int page, int size)
    {
        var query = _context.TeacherAttendances
            .Where(a => a.TeacherId == teacherId && !a.IsDeleted)
            .OrderByDescending(a => a.AttendanceDate);

        var total = await query.CountAsync();
        var items = await query.Skip((page - 1) * size).Take(size)
            .Select(a => new TeacherAttendanceDto
            {
                Id = a.Id,
                TeacherId = a.TeacherId,
                AttendanceDate = a.AttendanceDate,
                Status = a.Status,
                Remarks = a.Remarks
            })
            .ToListAsync();

        return new PagedResult<TeacherAttendanceDto> { Items = items, TotalItems = total, Page = page, PageSize = size };
    }

    // ── Leave ───────────────────────────────────────────────────────────────

    public async Task<PagedResult<TeacherLeaveDto>> GetLeavesPagedAsync(int page, int size, string? status = null)
    {
        var query = _context.TeacherLeaves
            .Include(l => l.Teacher)
            .Where(l => !l.IsDeleted && (string.IsNullOrEmpty(status) || l.Status == status))
            .OrderByDescending(l => l.CreatedAt);

        var total = await query.CountAsync();
        var items = await query.Skip((page - 1) * size).Take(size)
            .Select(l => new TeacherLeaveDto
            {
                Id = l.Id,
                TeacherId = l.TeacherProfileId,
                TeacherName = l.Teacher != null ? l.Teacher.FullName : "Unknown",
                LeaveType = l.LeaveType,
                StartDate = l.StartDate,
                EndDate = l.EndDate,
                Reason = l.Reason,
                Status = l.Status,
                ApproverRemarks = l.ApproverRemarks
            })
            .ToListAsync();

        return new PagedResult<TeacherLeaveDto> { Items = items, TotalItems = total, Page = page, PageSize = size };
    }

    public async Task RequestLeaveAsync(TeacherLeaveDto dto, string userId)
    {
        var leave = new TeacherLeave
        {
            TeacherProfileId = dto.TeacherId,
            LeaveType = dto.LeaveType,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Reason = dto.Reason,
            Status = "Pending",
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow
        };

        _context.TeacherLeaves.Add(leave);
        await _context.SaveChangesAsync();
    }

    public async Task ApproveLeaveAsync(int leaveId, string remarks, string userId)
    {
        var leave = await _context.TeacherLeaves.FindAsync(leaveId);
        if (leave == null) throw new Exception("Leave record not found.");

        leave.Status = "Approved";
        leave.ApproverRemarks = remarks;
        leave.ApprovedDate = DateTime.UtcNow;
        leave.UpdatedBy = userId;
        leave.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task RejectLeaveAsync(int leaveId, string remarks, string userId)
    {
        var leave = await _context.TeacherLeaves.FindAsync(leaveId);
        if (leave == null) throw new Exception("Leave record not found.");

        leave.Status = "Rejected";
        leave.ApproverRemarks = remarks;
        leave.UpdatedBy = userId;
        leave.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    // ── Payroll ──────────────────────────────────────────────────────────────

    public async Task<PagedResult<TeacherPayrollDto>> GetPayrollPagedAsync(int page, int size, DateTime? monthYear = null)
    {
        var query = _context.TeacherSalaries
            .Include(s => s.Teacher)
            .Where(s => !s.IsDeleted && (!monthYear.HasValue || (s.MonthYear.Month == monthYear.Value.Month && s.MonthYear.Year == monthYear.Value.Year)))
            .OrderByDescending(s => s.MonthYear);

        var total = await query.CountAsync();
        var items = await query.Skip((page - 1) * size).Take(size)
            .Select(s => new TeacherPayrollDto
            {
                Id = s.Id,
                TeacherId = s.TeacherProfileId,
                TeacherName = s.Teacher != null ? s.Teacher.FullName : "Unknown",
                MonthYear = s.MonthYear,
                BasicSalary = s.BasicSalary,
                Allowances = s.Allowances,
                Deductions = s.Deductions,
                NetSalary = s.NetSalary,
                Status = s.Status
            })
            .ToListAsync();

        return new PagedResult<TeacherPayrollDto> { Items = items, TotalItems = total, Page = page, PageSize = size };
    }

    public async Task GenerateMonthlyPayrollAsync(DateTime monthYear, string userId)
    {
        var activeTeachers = await _context.Teachers
            .Where(t => !t.IsDeleted && t.Status == SchoolManagementSystem.Models.Enums.TeacherStatus.Active)
            .ToListAsync();

        foreach (var teacher in activeTeachers)
        {
            var existing = await _context.TeacherSalaries
                .AnyAsync(s => s.TeacherProfileId == teacher.Id && s.MonthYear.Month == monthYear.Month && s.MonthYear.Year == monthYear.Year && !s.IsDeleted);

            if (!existing)
            {
                // Placeholder logic for salary calculation
                // In a real app, this would fetch from a TeacherSalaryStructure entity
                var salary = new TeacherSalary
                {
                    TeacherProfileId = teacher.Id,
                    MonthYear = new DateTime(monthYear.Year, monthYear.Month, 1),
                    BasicSalary = 30000, // Default
                    Allowances = 5000,
                    Deductions = 0,
                    NetSalary = 35000,
                    Status = "Unpaid",
                    CreatedBy = userId,
                    CreatedAt = DateTime.UtcNow
                };
                _context.TeacherSalaries.Add(salary);
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task UpdatePayrollStatusAsync(int payrollId, string status, string userId)
    {
        var salary = await _context.TeacherSalaries.FindAsync(payrollId);
        if (salary == null) throw new Exception("Payroll record not found.");

        salary.Status = status;
        salary.UpdatedBy = userId;
        salary.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }
}
