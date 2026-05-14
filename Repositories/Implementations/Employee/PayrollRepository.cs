using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Employee;
using SchoolManagementSystem.Repositories.Interfaces.Employee;

namespace SchoolManagementSystem.Repositories.Implementations.Employee;

public class EmployeePayrollRepository : BaseRepository<EmployeePayroll>, IEmployeePayrollRepository
{
    public EmployeePayrollRepository(SchoolDbContext db) : base(db) { }

    public async Task<IEnumerable<EmployeePayroll>> GetEmployeePayrollHistoryAsync(long employeeId, CancellationToken ct = default)
    {
        return await _db.EmployeePayrolls
            .Include(p => p.GeneratedBy)
            .Include(p => p.ApprovedBy)
            .Where(p => p.EmployeeId == employeeId)
            .OrderByDescending(p => p.PayrollYear)
            .ThenByDescending(p => p.PayrollMonth)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<EmployeePayroll?> GetByMonthYearAsync(long employeeId, int month, int year, CancellationToken ct = default)
    {
        return await _db.EmployeePayrolls
            .FirstOrDefaultAsync(p => p.EmployeeId == employeeId && p.PayrollMonth == month && p.PayrollYear == year, ct);
    }

    public async Task<bool> ExistsAsync(long employeeId, int month, int year, CancellationToken ct = default)
    {
        return await _db.EmployeePayrolls
            .AnyAsync(p => p.EmployeeId == employeeId && p.PayrollMonth == month && p.PayrollYear == year && p.PaymentStatus != SchoolManagementSystem.Models.Enums.PayrollPaymentStatus.Cancelled, ct);
    }
}

public class SalaryStructureRepository : BaseRepository<EmployeeSalaryStructure>, ISalaryStructureRepository
{
    public SalaryStructureRepository(SchoolDbContext db) : base(db) { }

    public async Task<EmployeeSalaryStructure?> GetActiveStructureAsync(long employeeId, CancellationToken ct = default)
    {
        return await _db.EmployeeSalaryStructures
            .Where(s => s.EmployeeId == employeeId && s.IsActive)
            .OrderByDescending(s => s.EffectiveFrom)
            .FirstOrDefaultAsync(ct);
    }
}
