using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Employee;
using SchoolManagementSystem.Models.DTOs.Employee;
using SchoolManagementSystem.Repositories.Interfaces.Employee;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using SchoolManagementSystem.Models.DTOs.Common;

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
        var connection = _db.Database.GetDbConnection();
        
        var parameters = new DynamicParameters();
        parameters.Add("@PageNumber", page);
        parameters.Add("@PageSize", pageSize);
        parameters.Add("@Search", search);
        parameters.Add("@DepartmentId", departmentId);
        parameters.Add("@DesignationId", designationId);
        parameters.Add("@Status", isActive.HasValue ? (isActive.Value ? 1 : 0) : null);
        parameters.Add("@SortField", "Id"); 
        parameters.Add("@SortDirection", "DESC");

        var result = (await connection.QueryAsync<dynamic>(
            "sp_Employee_GetPaged",
            parameters,
            commandType: CommandType.StoredProcedure
        )).ToList();

        var data = result.Select(x => new EmployeeListItemDto
        {
            Id = Convert.ToInt64(x.Id),
            EmployeeCode = x.EmployeeCode,
            FullName = x.FullName,
            Phone = x.Phone,
            DepartmentName = x.DepartmentName,
            DesignationName = x.DesignationName,
            IsActive = (bool)x.Status,
            PhotoPath = null 
        }).ToList();

        int totalRecords = data.Any() ? (int)result.First().TotalCount : 0;

        return (data, totalRecords);
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
