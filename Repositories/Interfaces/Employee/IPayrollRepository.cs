using SchoolManagementSystem.Models.Entities.Employee;
using SchoolManagementSystem.Repositories.Interfaces;
using SchoolManagementSystem.Models.DTOs.Employee;

namespace SchoolManagementSystem.Repositories.Interfaces.Employee;

public interface IEmployeePayrollRepository : IBaseRepository<EmployeePayroll>
{
    Task<IEnumerable<EmployeePayroll>> GetEmployeePayrollHistoryAsync(long employeeId, CancellationToken ct = default);
    Task<EmployeePayroll?> GetByMonthYearAsync(long employeeId, int month, int year, CancellationToken ct = default);
    Task<bool> ExistsAsync(long employeeId, int month, int year, CancellationToken ct = default);
    Task<(List<EmployeePayrollDto> items, int totalRecords)> GetPagedAsync(
        int page, int pageSize, string? search, long? departmentId, int? status, 
        int? month, int? year, CancellationToken ct);
}

public interface ISalaryStructureRepository : IBaseRepository<EmployeeSalaryStructure>
{
    Task<EmployeeSalaryStructure?> GetActiveStructureAsync(long employeeId, CancellationToken ct = default);
}
