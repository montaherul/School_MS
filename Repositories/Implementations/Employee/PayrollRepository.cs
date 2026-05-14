using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Employee;
using SchoolManagementSystem.Repositories.Interfaces.Employee;
using Dapper;
using System.Data;
using SchoolManagementSystem.Models.DTOs.Employee;

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

    public async Task<(List<EmployeePayrollDto> items, int totalRecords)> GetPagedAsync(
        int page, int pageSize, string? search, long? departmentId, int? status, 
        int? month, int? year, CancellationToken ct)
    {
        var connection = _db.Database.GetDbConnection();
        
        var parameters = new DynamicParameters();
        parameters.Add("@PageNumber", page);
        parameters.Add("@PageSize", pageSize);
        parameters.Add("@Search", search);
        parameters.Add("@DepartmentId", departmentId);
        parameters.Add("@Status", status);
        parameters.Add("@Month", month);
        parameters.Add("@Year", year);
        parameters.Add("@SortField", "PayrollYear");
        parameters.Add("@SortDirection", "DESC");

        var result = (await connection.QueryAsync<dynamic>(
            "sp_Payroll_GetPaged",
            parameters,
            commandType: CommandType.StoredProcedure
        )).ToList();

        var data = result.Select(x => new EmployeePayrollDto
        {
            Id = (long)x.Id,
            EmployeeId = (long)x.EmployeeId,
            EmployeeName = x.FullName,
            EmployeeCode = x.EmployeeCode,
            DepartmentName = x.DepartmentName,
            DesignationName = x.DesignationName,
            PayrollMonth = x.PayrollMonth,
            PayrollYear = x.PayrollYear,
            WorkingDays = x.WorkingDays,
            PresentDays = x.PresentDays,
            GrossSalary = x.GrossSalary,
            NetSalary = x.NetSalary,
            PaymentStatus = (SchoolManagementSystem.Models.Enums.PayrollPaymentStatus)x.PaymentStatus,
            PaymentDate = x.PaymentDate
        }).ToList();

        int totalRecords = data.Any() ? (int)result.First().TotalCount : 0;

        return (data, totalRecords);
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
