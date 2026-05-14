using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.DTOs.Employee;

namespace SchoolManagementSystem.Services.Interfaces.Employee;

public interface IEmployeePayrollService
{
    Task<int> GeneratePayrollAsync(int month, int year, long? departmentId, long generatedByUserId, CancellationToken ct = default);
    Task ApprovePayrollAsync(long payrollId, long approvedByUserId, CancellationToken ct = default);
    Task MarkAsPaidAsync(long payrollId, DateTime paymentDate, string? remarks, long updatedByUserId, CancellationToken ct = default);
    Task CancelPayrollAsync(long payrollId, long updatedByUserId, CancellationToken ct = default);
    
    Task<PagedResult<EmployeePayrollDto>> GetPagedAsync(int page, int pageSize, int month, int year, long? departmentId, SchoolManagementSystem.Models.Enums.PayrollPaymentStatus? status, CancellationToken ct = default);
    Task<EmployeePayrollDto?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<IEnumerable<EmployeePayrollDto>> GetEmployeeHistoryAsync(long employeeId, CancellationToken ct = default);
    Task<IEnumerable<EmployeePayrollDto>> GetRecentByEmployeeIdAsync(long employeeId, int count, CancellationToken ct = default);
    Task<PayrollSummaryDto> GetDashboardSummaryAsync(int month, int year, CancellationToken ct = default);
}

public interface ISalaryStructureService
{
    Task<long> CreateAsync(SalaryStructureDto dto, long createdByUserId, CancellationToken ct = default);
    Task<SalaryStructureDto?> GetActiveByEmployeeIdAsync(long employeeId, CancellationToken ct = default);
    Task<IEnumerable<SalaryStructureDto>> GetHistoryByEmployeeIdAsync(long employeeId, CancellationToken ct = default);
}
