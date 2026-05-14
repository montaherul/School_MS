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

public class EmployeePayrollService : IEmployeePayrollService
{
    private readonly IUnitOfWork _uow;
    private readonly IEmployeePayrollRepository _payrollRepo;
    private readonly ISalaryStructureRepository _structureRepo;
    private readonly IEmployeeRepository _employeeRepo;
    private readonly IEmployeeAttendanceRepository _attendanceRepo;
    private readonly IEmployeeLeaveRepository _leaveRepo;
    private readonly IAuditLogRepository _auditLogRepo;

    public EmployeePayrollService(
        IUnitOfWork uow,
        IEmployeePayrollRepository payrollRepo,
        ISalaryStructureRepository structureRepo,
        IEmployeeRepository employeeRepo,
        IEmployeeAttendanceRepository attendanceRepo,
        IEmployeeLeaveRepository leaveRepo,
        IAuditLogRepository auditLogRepo)
    {
        _uow = uow;
        _payrollRepo = payrollRepo;
        _structureRepo = structureRepo;
        _employeeRepo = employeeRepo;
        _attendanceRepo = attendanceRepo;
        _leaveRepo = leaveRepo;
        _auditLogRepo = auditLogRepo;
    }

    public async Task<int> GeneratePayrollAsync(int month, int year, long? departmentId, long generatedByUserId, CancellationToken ct = default)
    {
        var employeesQuery = _employeeRepo.Query().Where(e => e.IsActive);
        if (departmentId.HasValue) employeesQuery = employeesQuery.Where(e => e.DepartmentId == departmentId.Value);

        var employees = await employeesQuery.ToListAsync(ct);
        int generatedCount = 0;

        DateTime monthStart = new DateTime(year, month, 1);
        DateTime monthEnd = monthStart.AddMonths(1).AddDays(-1);
        int workingDays = 0;
        for (var d = monthStart; d <= monthEnd; d = d.AddDays(1))
        {
            if (d.DayOfWeek != DayOfWeek.Friday) workingDays++; // Simplified working days calc
        }

        foreach (var emp in employees)
        {
            if (await _payrollRepo.ExistsAsync(emp.Id, month, year, ct)) continue;

            var structure = await _structureRepo.GetActiveStructureAsync(emp.Id, ct);
            if (structure == null) continue; // Skip employees without a salary structure

            // Fetch Attendance Stats
            var attendance = await _attendanceRepo.Query()
                .Where(a => a.EmployeeId == emp.Id && a.AttendanceDate >= monthStart && a.AttendanceDate <= monthEnd)
                .ToListAsync(ct);

            int present = attendance.Count(a => a.Status == AttendanceStatus.Present);
            int absent = attendance.Count(a => a.Status == AttendanceStatus.Absent);
            int leave = attendance.Count(a => a.Status == AttendanceStatus.Leave);
            int late = attendance.Count(a => a.Status == AttendanceStatus.Late);

            // Fetch Leave Stats to distinguish paid/unpaid
            var leaves = await _leaveRepo.Query()
                .Include(l => l.LeaveType)
                .Where(l => l.EmployeeId == emp.Id && l.Status == LeaveStatus.Approved && l.StartDate <= monthEnd && l.EndDate >= monthStart)
                .ToListAsync(ct);

            int paidLeave = 0;
            int unpaidLeave = 0;
            foreach (var l in leaves)
            {
                // Calculate overlapping days with the current month
                var start = l.StartDate < monthStart ? monthStart : l.StartDate;
                var end = l.EndDate > monthEnd ? monthEnd : l.EndDate;
                int days = (end.Date - start.Date).Days + 1;

                if (l.LeaveType.IsPaid) paidLeave += days;
                else unpaidLeave += days;
            }

            // Salary Calculation
            decimal perDaySalary = structure.BasicSalary / workingDays;
            decimal absenceDeduction = (absent + unpaidLeave) * perDaySalary;
            
            // Tax and PF
            decimal taxAmount = (structure.BasicSalary + structure.HouseRent + structure.MedicalAllowance + structure.TransportAllowance + structure.OtherAllowance) * (structure.TaxPercentage / 100);
            decimal grossSalary = structure.BasicSalary + structure.HouseRent + structure.MedicalAllowance + structure.TransportAllowance + structure.OtherAllowance;
            decimal netSalary = grossSalary - absenceDeduction - structure.ProvidentFund - taxAmount;

            var payroll = new EmployeePayroll
            {
                EmployeeId = emp.Id,
                PayrollMonth = month,
                PayrollYear = year,
                WorkingDays = workingDays,
                PresentDays = present,
                AbsentDays = absent,
                LeaveDays = leave,
                PaidLeaveDays = paidLeave,
                UnpaidLeaveDays = unpaidLeave,
                LateDays = late,
                GrossSalary = grossSalary,
                DeductionAmount = absenceDeduction + structure.ProvidentFund + taxAmount,
                NetSalary = netSalary,
                PaymentStatus = PayrollPaymentStatus.Pending,
                GeneratedById = generatedByUserId,
                GeneratedAt = DateTime.UtcNow
            };

            await _payrollRepo.AddAsync(payroll, ct);
            generatedCount++;
        }

        if (generatedCount > 0)
        {
            await _auditLogRepo.AddAsync(new SchoolManagementSystem.Models.Entities.Auth.AuditLog
            {
                Module = "Payroll",
                Action = "Generate",
                Details = $"Payroll generated for {month}/{year}, Count: {generatedCount}",
                UserId = (int)generatedByUserId,
                CreatedAt = DateTime.UtcNow
            }, ct);

            await _uow.SaveChangesAsync(ct);
        }

        return generatedCount;
    }

    public async Task ApprovePayrollAsync(long payrollId, long approvedByUserId, CancellationToken ct = default)
    {
        var payroll = await _payrollRepo.FirstOrDefaultAsync(p => p.Id == payrollId, ct)
            ?? throw new KeyNotFoundException("Payroll record not found.");

        if (payroll.PaymentStatus != PayrollPaymentStatus.Pending)
            throw new InvalidOperationException("Only pending payroll can be approved.");

        payroll.PaymentStatus = PayrollPaymentStatus.Approved;
        payroll.ApprovedById = approvedByUserId;
        payroll.ApprovedAt = DateTime.UtcNow;

        _payrollRepo.Update(payroll);
        
        await _auditLogRepo.AddAsync(new SchoolManagementSystem.Models.Entities.Auth.AuditLog
        {
            Module = "Payroll",
            Action = "Approve",
            Details = $"Payroll approved for ID: {payrollId}",
            UserId = (int)approvedByUserId,
            CreatedAt = DateTime.UtcNow
        }, ct);

        await _uow.SaveChangesAsync(ct);
    }

    public async Task MarkAsPaidAsync(long payrollId, DateTime paymentDate, string? remarks, long updatedByUserId, CancellationToken ct = default)
    {
        var payroll = await _payrollRepo.FirstOrDefaultAsync(p => p.Id == payrollId, ct)
            ?? throw new KeyNotFoundException("Payroll record not found.");

        if (payroll.PaymentStatus != PayrollPaymentStatus.Approved)
            throw new InvalidOperationException("Only approved payroll can be marked as paid.");

        payroll.PaymentStatus = PayrollPaymentStatus.Paid;
        payroll.PaymentDate = paymentDate;
        payroll.Remarks = remarks;

        _payrollRepo.Update(payroll);

        await _auditLogRepo.AddAsync(new SchoolManagementSystem.Models.Entities.Auth.AuditLog
        {
            Module = "Payroll",
            Action = "Paid",
            Details = $"Salary paid for ID: {payrollId}, Date: {paymentDate:yyyy-MM-dd}",
            UserId = (int)updatedByUserId,
            CreatedAt = DateTime.UtcNow
        }, ct);

        await _uow.SaveChangesAsync(ct);
    }

    public async Task CancelPayrollAsync(long payrollId, long updatedByUserId, CancellationToken ct = default)
    {
        var payroll = await _payrollRepo.FirstOrDefaultAsync(p => p.Id == payrollId, ct)
            ?? throw new KeyNotFoundException("Payroll record not found.");

        if (payroll.PaymentStatus == PayrollPaymentStatus.Paid)
            throw new InvalidOperationException("Paid payroll cannot be cancelled.");

        payroll.PaymentStatus = PayrollPaymentStatus.Cancelled;
        _payrollRepo.Update(payroll);

        await _auditLogRepo.AddAsync(new SchoolManagementSystem.Models.Entities.Auth.AuditLog
        {
            Module = "Payroll",
            Action = "Cancel",
            Details = $"Payroll cancelled for ID: {payrollId}",
            UserId = (int)updatedByUserId,
            CreatedAt = DateTime.UtcNow
        }, ct);

        await _uow.SaveChangesAsync(ct);
    }

    public async Task<PagedResult<EmployeePayrollDto>> GetPagedAsync(int page, int pageSize, int month, int year, long? departmentId, PayrollPaymentStatus? status, CancellationToken ct = default)
    {
        var query = _payrollRepo.Query()
            .Include(p => p.Employee).ThenInclude(e => e.Department)
            .Include(p => p.Employee).ThenInclude(e => e.Designation)
            .Include(p => p.GeneratedBy)
            .Include(p => p.ApprovedBy)
            .Where(p => p.PayrollMonth == month && p.PayrollYear == year);

        if (departmentId.HasValue) query = query.Where(p => p.Employee.DepartmentId == departmentId.Value);
        if (status.HasValue) query = query.Where(p => p.PaymentStatus == status.Value);

        var totalItems = await query.CountAsync(ct);
        var items = await query.OrderBy(p => p.Employee.FullName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new EmployeePayrollDto
            {
                Id = p.Id,
                EmployeeId = p.EmployeeId,
                EmployeeName = p.Employee.FullName,
                EmployeeCode = p.Employee.EmployeeCode,
                DepartmentName = p.Employee.Department.Name,
                DesignationName = p.Employee.Designation.Name,
                PayrollMonth = p.PayrollMonth,
                PayrollYear = p.PayrollYear,
                WorkingDays = p.WorkingDays,
                PresentDays = p.PresentDays,
                AbsentDays = p.AbsentDays,
                LeaveDays = p.LeaveDays,
                PaidLeaveDays = p.PaidLeaveDays,
                UnpaidLeaveDays = p.UnpaidLeaveDays,
                LateDays = p.LateDays,
                OvertimeHours = p.OvertimeHours,
                BonusAmount = p.BonusAmount,
                DeductionAmount = p.DeductionAmount,
                GrossSalary = p.GrossSalary,
                NetSalary = p.NetSalary,
                PaymentStatus = p.PaymentStatus,
                PaymentDate = p.PaymentDate,
                Remarks = p.Remarks,
                GeneratedByName = p.GeneratedBy != null ? p.GeneratedBy.UserName : null,
                GeneratedAt = p.GeneratedAt,
                ApprovedByName = p.ApprovedBy != null ? p.ApprovedBy.UserName : null,
                ApprovedAt = p.ApprovedAt
            })
            .ToListAsync(ct);

        return new PagedResult<EmployeePayrollDto> { Items = items, TotalItems = totalItems, Page = page, PageSize = pageSize };
    }

    public async Task<EmployeePayrollDto?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        return await _payrollRepo.Query()
            .Include(p => p.Employee).ThenInclude(e => e.Department)
            .Include(p => p.Employee).ThenInclude(e => e.Designation)
            .Include(p => p.GeneratedBy)
            .Include(p => p.ApprovedBy)
            .Where(p => p.Id == id)
            .Select(p => new EmployeePayrollDto
            {
                Id = p.Id,
                EmployeeId = p.EmployeeId,
                EmployeeName = p.Employee.FullName,
                EmployeeCode = p.Employee.EmployeeCode,
                DepartmentName = p.Employee.Department.Name,
                DesignationName = p.Employee.Designation.Name,
                PayrollMonth = p.PayrollMonth,
                PayrollYear = p.PayrollYear,
                NetSalary = p.NetSalary,
                PaymentStatus = p.PaymentStatus,
                GeneratedAt = p.GeneratedAt
                // Add more fields if needed for payslip
            })
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IEnumerable<EmployeePayrollDto>> GetEmployeeHistoryAsync(long employeeId, CancellationToken ct = default)
    {
        return await _payrollRepo.Query()
            .Where(p => p.EmployeeId == employeeId)
            .OrderByDescending(p => p.PayrollYear).ThenByDescending(p => p.PayrollMonth)
            .Select(p => new EmployeePayrollDto
            {
                Id = p.Id,
                PayrollMonth = p.PayrollMonth,
                PayrollYear = p.PayrollYear,
                NetSalary = p.NetSalary,
                PaymentStatus = p.PaymentStatus,
                PaymentDate = p.PaymentDate
            })
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<EmployeePayrollDto>> GetRecentByEmployeeIdAsync(long employeeId, int count, CancellationToken ct = default)
    {
        return await _payrollRepo.Query()
            .Where(p => p.EmployeeId == employeeId)
            .OrderByDescending(p => p.PayrollYear).ThenByDescending(p => p.PayrollMonth)
            .Take(count)
            .Select(p => new EmployeePayrollDto
            {
                Id = p.Id,
                PayrollMonth = p.PayrollMonth,
                PayrollYear = p.PayrollYear,
                NetSalary = p.NetSalary,
                PaymentStatus = p.PaymentStatus,
                PaymentDate = p.PaymentDate
            })
            .ToListAsync(ct);
    }

    public async Task<PayrollSummaryDto> GetDashboardSummaryAsync(int month, int year, CancellationToken ct = default)
    {
        var payrolls = await _payrollRepo.Query()
            .Where(p => p.PayrollMonth == month && p.PayrollYear == year && p.PaymentStatus != PayrollPaymentStatus.Cancelled)
            .ToListAsync(ct);

        return new PayrollSummaryDto
        {
            TotalExpense = payrolls.Sum(p => p.NetSalary),
            TotalPaid = payrolls.Count(p => p.PaymentStatus == PayrollPaymentStatus.Paid),
            TotalPending = payrolls.Count(p => p.PaymentStatus == PayrollPaymentStatus.Pending || p.PaymentStatus == PayrollPaymentStatus.Approved),
            AverageSalary = payrolls.Any() ? payrolls.Average(p => p.NetSalary) : 0
        };
    }
}

public class SalaryStructureService : ISalaryStructureService
{
    private readonly IUnitOfWork _uow;
    private readonly ISalaryStructureRepository _repo;

    public SalaryStructureService(IUnitOfWork uow, ISalaryStructureRepository repo)
    {
        _uow = uow;
        _repo = repo;
    }

    public async Task<long> CreateAsync(SalaryStructureDto dto, long createdByUserId, CancellationToken ct = default)
    {
        // Deactivate existing structure
        var existing = await _repo.Query().Where(s => s.EmployeeId == dto.EmployeeId && s.IsActive).ToListAsync(ct);
        foreach (var s in existing)
        {
            s.IsActive = false;
            _repo.Update(s);
        }

        var structure = new EmployeeSalaryStructure
        {
            EmployeeId = dto.EmployeeId,
            BasicSalary = dto.BasicSalary,
            HouseRent = dto.HouseRent,
            MedicalAllowance = dto.MedicalAllowance,
            TransportAllowance = dto.TransportAllowance,
            OtherAllowance = dto.OtherAllowance,
            TaxPercentage = dto.TaxPercentage,
            ProvidentFund = dto.ProvidentFund,
            EffectiveFrom = dto.EffectiveFrom,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(structure, ct);
        await _uow.SaveChangesAsync(ct);
        return structure.Id;
    }

    public async Task<SalaryStructureDto?> GetActiveByEmployeeIdAsync(long employeeId, CancellationToken ct = default)
    {
        var s = await _repo.GetActiveStructureAsync(employeeId, ct);
        if (s == null) return null;

        return new SalaryStructureDto
        {
            Id = s.Id,
            EmployeeId = s.EmployeeId,
            BasicSalary = s.BasicSalary,
            HouseRent = s.HouseRent,
            MedicalAllowance = s.MedicalAllowance,
            TransportAllowance = s.TransportAllowance,
            OtherAllowance = s.OtherAllowance,
            TaxPercentage = s.TaxPercentage,
            ProvidentFund = s.ProvidentFund,
            EffectiveFrom = s.EffectiveFrom,
            IsActive = s.IsActive
        };
    }

    public async Task<IEnumerable<SalaryStructureDto>> GetHistoryByEmployeeIdAsync(long employeeId, CancellationToken ct = default)
    {
        return await _repo.Query()
            .Where(s => s.EmployeeId == employeeId)
            .OrderByDescending(s => s.EffectiveFrom)
            .Select(s => new SalaryStructureDto
            {
                Id = s.Id,
                BasicSalary = s.BasicSalary,
                EffectiveFrom = s.EffectiveFrom,
                IsActive = s.IsActive
            })
            .ToListAsync(ct);
    }
}
