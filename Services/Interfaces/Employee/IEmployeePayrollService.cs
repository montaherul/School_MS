using SchoolManagementSystem.Models.DTOs.Employee;

namespace SchoolManagementSystem.Services.Interfaces.Employee;

public interface IEmployeePayrollService
{
    Task<List<EmployeeSalaryDto>> GetSalariesByEmployeeIdAsync(int employeeId, CancellationToken ct);
    Task<EmployeeSalaryDto?> GetSalaryByIdAsync(int id, CancellationToken ct);
    Task SaveSalaryAsync(EmployeeSalaryDto dto, CancellationToken ct);
    Task DeleteSalaryAsync(int id, CancellationToken ct);
}
