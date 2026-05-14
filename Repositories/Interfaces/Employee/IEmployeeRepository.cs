using SchoolManagementSystem.Models.Entities.Employee;
using SchoolManagementSystem.Models.DTOs.Employee;

namespace SchoolManagementSystem.Repositories.Interfaces.Employee;

public interface IEmployeeRepository : IBaseRepository<SchoolManagementSystem.Models.Entities.Employee.Employee>
{
    Task<SchoolManagementSystem.Models.Entities.Employee.Employee?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<(List<EmployeeListItemDto> items, int totalRecords)> GetPagedAsync(int page, int pageSize, string? search, long? departmentId, long? designationId, bool? isActive, CancellationToken ct);
    Task<SchoolManagementSystem.Models.Entities.Employee.Employee?> GetByUserIdAsync(long userId, CancellationToken ct = default);
}


public interface IDepartmentRepository : IBaseRepository<Department>
{
    Task<Department?> GetByIdAsync(long id, CancellationToken ct = default);
}

public interface IDesignationRepository : IBaseRepository<Designation>
{
    Task<Designation?> GetByIdAsync(long id, CancellationToken ct = default);
}
