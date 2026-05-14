using SchoolManagementSystem.Models.DTOs.Employee;
using SchoolManagementSystem.Models.ViewModels.Employee;
using SchoolManagementSystem.Models.DTOs.Common;

namespace SchoolManagementSystem.Services.Interfaces.Employee;

public interface IEmployeeService
{
    Task<PagedResult<EmployeeListItemDto>> GetPagedAsync(int page, int pageSize, string? search, long? departmentId = null, long? designationId = null, bool? isActive = null, CancellationToken ct = default);
    Task<EmployeeViewModel?> GetForEditAsync(long id, CancellationToken ct = default);
    Task<long> CreateAsync(EmployeeViewModel model, string createdBy, CancellationToken ct = default);
    Task UpdateAsync(EmployeeViewModel model, string updatedBy, CancellationToken ct = default);
    Task DeleteAsync(long id, string updatedBy, CancellationToken ct = default);
    Task ToggleAccessAsync(long id, string updatedBy, CancellationToken ct = default);
    Task<EmployeeViewModel?> GetDetailAsync(long id, CancellationToken ct = default);
    Task<long?> GetEmployeeIdByUserIdAsync(long userId, CancellationToken ct = default);
}

public interface IDepartmentService
{
    Task<IEnumerable<DepartmentDto>> GetAllAsync(CancellationToken ct = default);
    Task<DepartmentViewModel?> GetByIdAsync(long id, CancellationToken ct = default);
    Task CreateAsync(DepartmentViewModel model, string createdBy, CancellationToken ct = default);
    Task UpdateAsync(DepartmentViewModel model, string updatedBy, CancellationToken ct = default);
    Task DeleteAsync(long id, string updatedBy, CancellationToken ct = default);
}

public interface IDesignationService
{
    Task<IEnumerable<DesignationDto>> GetAllAsync(CancellationToken ct = default);
    Task<DesignationViewModel?> GetByIdAsync(long id, CancellationToken ct = default);
    Task CreateAsync(DesignationViewModel model, string createdBy, CancellationToken ct = default);
    Task UpdateAsync(DesignationViewModel model, string updatedBy, CancellationToken ct = default);
    Task DeleteAsync(long id, string updatedBy, CancellationToken ct = default);
}
