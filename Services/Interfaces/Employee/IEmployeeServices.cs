using SchoolManagementSystem.Models.DTOs.Employee;
using SchoolManagementSystem.Models.Entities.Academic;

namespace SchoolManagementSystem.Services.Interfaces.Employee;

public interface IEmployeeService
{
    Task<(List<EmployeeListItemDto> items, int totalRecords)> GetPagedAsync(int page, int pageSize, string? search, int? departmentId, int? designationId, bool? isTeachingStaff, string? status, CancellationToken ct);
    Task<EmployeeUpsertDto?> GetForEditAsync(int id, CancellationToken ct);
    Task<EmployeeDetailsDto?> GetDetailsAsync(int id, CancellationToken ct);
    Task<EmployeeUpsertDto?> GetByUserIdAsync(int userId, CancellationToken ct);
    Task<int> SaveAsync(EmployeeUpsertDto dto, CancellationToken ct);
    Task<bool> DeleteAsync(int id, CancellationToken ct);
    Task<bool> UpdateStatusAsync(int id, string status, CancellationToken ct);
    Task<bool> IsCodeExistsAsync(string code, int? excludeId, CancellationToken ct);
    Task<bool> IsEmailExistsAsync(string email, int? excludeId, CancellationToken ct);
    Task<bool> IsPhoneExistsAsync(string phone, int? excludeId, CancellationToken ct);
    Task<EmployeeDashboardDto> GetDashboardAsync(CancellationToken ct);
}

public interface IDepartmentService
{
    Task<IReadOnlyList<DepartmentDto>> GetAllAsync(CancellationToken ct);
}

public interface IDesignationService
{
    Task<IReadOnlyList<DesignationDto>> GetAllAsync(CancellationToken ct);
}

public interface IUserProvisionService
{
    Task<(int userId, string username, string password)> ProvisionUserForEmployeeAsync(SchoolManagementSystem.Models.Entities.Employee.Employee employee, CancellationToken ct);
}
