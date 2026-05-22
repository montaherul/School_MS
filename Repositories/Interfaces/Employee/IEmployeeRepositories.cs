using SchoolManagementSystem.Models.Entities.Employee;
using SchoolManagementSystem.Models.DTOs.Employee;

namespace SchoolManagementSystem.Repositories.Interfaces.Employee;

public interface IEmployeeRepository : IBaseRepository<SchoolManagementSystem.Models.Entities.Employee.Employee>
{
    Task<(List<EmployeeListItemDto> items, int totalRecords)> GetPagedAsync(int page, int pageSize, string? search, int? departmentId, int? designationId, bool? isTeachingStaff, string? status, CancellationToken ct);
    Task<EmployeeUpsertDto?> GetForEditAsync(int id, CancellationToken ct);
    Task<EmployeeDetailsDto?> GetDetailsAsync(int id, CancellationToken ct);
    Task<EmployeeUpsertDto?> GetByUserIdAsync(int userId, CancellationToken ct);
}

public interface IDepartmentRepository : IBaseRepository<Department>
{
}

public interface IDesignationRepository : IBaseRepository<Designation>
{
}

public interface IEmployeeQualificationRepository : IBaseRepository<EmployeeQualification>
{
}

public interface IEmployeeDocumentRepository : IBaseRepository<EmployeeDocument>
{
}

public interface IEmployeeExperienceRepository : IBaseRepository<EmployeeExperience>
{
}

public interface IEmployeeInvitationRepository : IBaseRepository<EmployeeInvitation>
{
    Task<(List<EmployeeInvitationDto> items, int totalRecords)> GetPagedBySpAsync(
        int page, int pageSize, string? search, CancellationToken ct);
}
