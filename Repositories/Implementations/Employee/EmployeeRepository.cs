using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Employee;
using SchoolManagementSystem.Models.DTOs.Employee;
using SchoolManagementSystem.Repositories.Interfaces.Employee;

namespace SchoolManagementSystem.Repositories.Implementations.Employee;

public class EmployeeRepository : BaseRepository<SchoolManagementSystem.Models.Entities.Employee.Employee>, IEmployeeRepository
{
    public EmployeeRepository(SchoolDbContext db) : base(db) { }

    public async Task<SchoolManagementSystem.Models.Entities.Employee.Employee?> GetByIdAsync(long id, CancellationToken ct = default)

    {
        return await _db.Employees
            .Include(e => e.Department)
            .Include(e => e.Designation)
            .FirstOrDefaultAsync(e => e.Id == id, ct);
    }

    public async Task<(List<EmployeeListItemDto> items, int totalRecords)> GetPagedAsync(
        int page, int pageSize, string? search, long? departmentId, long? designationId, bool? isActive, CancellationToken ct)
    {
        var query = _db.Employees
            .Include(e => e.Department)
            .Include(e => e.Designation)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(e => e.FullName.Contains(search) || e.EmployeeCode.Contains(search) || e.Phone.Contains(search));
        }

        if (departmentId.HasValue) query = query.Where(e => e.DepartmentId == departmentId);
        if (designationId.HasValue) query = query.Where(e => e.DesignationId == designationId);
        if (isActive.HasValue) query = query.Where(e => e.IsActive == isActive);

        int totalRecords = await query.CountAsync(ct);
        var items = await query.OrderByDescending(e => e.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(e => new EmployeeListItemDto
            {
                Id = e.Id,
                EmployeeCode = e.EmployeeCode,
                FullName = e.FullName,
                Phone = e.Phone,
                DepartmentName = e.Department.Name,
                DesignationName = e.Designation.Name,
                IsActive = e.IsActive,
                PhotoPath = e.PhotoPath
            })
            .ToListAsync(ct);

        return (items, totalRecords);
    }
    public async Task<SchoolManagementSystem.Models.Entities.Employee.Employee?> GetByUserIdAsync(long userId, CancellationToken ct = default)
    {
        return await _db.Employees
            .Include(e => e.Department)
            .Include(e => e.Designation)
            .FirstOrDefaultAsync(e => e.User.Id == (int)userId, ct);
    }
}

public class DepartmentRepository : BaseRepository<Department>, IDepartmentRepository
{
    public DepartmentRepository(SchoolDbContext db) : base(db) { }
    public async Task<Department?> GetByIdAsync(long id, CancellationToken ct = default)
        => await _db.Departments.FindAsync(new object[] { id }, ct);
}

public class DesignationRepository : BaseRepository<Designation>, IDesignationRepository
{
    public DesignationRepository(SchoolDbContext db) : base(db) { }
    public async Task<Designation?> GetByIdAsync(long id, CancellationToken ct = default)
        => await _db.Designations.FindAsync(new object[] { id }, ct);
}
