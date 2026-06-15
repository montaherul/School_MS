using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Identity;
using SchoolManagementSystem.Repositories.Interfaces.Identity;

namespace SchoolManagementSystem.Repositories.Implementations.Identity;

public class IdCardRepository : IIdCardRepository
{
    private readonly SchoolDbContext _db;

    public IdCardRepository(SchoolDbContext db)
    {
        _db = db;
    }

    public async Task<(List<StudentIdCardListDto> Items, int TotalRecords)> GetStudentIdCardListAsync(
        int page, int pageSize, string? search,
        int? classId, int? sectionId, int? groupId,
        string? status, string? gender,
        DateTime? admissionFrom, DateTime? admissionTo,
        CancellationToken ct)
    {
        var parameters = new[]
        {
            new SqlParameter("@PageNumber", page),
            new SqlParameter("@PageSize", pageSize),
            new SqlParameter("@SearchTerm", (object?)search ?? DBNull.Value),
            new SqlParameter("@ClassId", classId ?? 0),
            new SqlParameter("@SectionId", sectionId ?? 0),
            new SqlParameter("@GroupId", groupId ?? 0),
            new SqlParameter("@Status", (object?)status ?? DBNull.Value),
            new SqlParameter("@Gender", (object?)gender ?? DBNull.Value),
            new SqlParameter("@AdmissionFrom", (object?)admissionFrom ?? DBNull.Value),
            new SqlParameter("@AdmissionTo", (object?)admissionTo ?? DBNull.Value)
        };

        var items = await _db.Set<StudentIdCardListDto>()
            .FromSqlRaw("EXEC sp_GetStudentIdCardList @PageNumber, @PageSize, @SearchTerm, @ClassId, @SectionId, @GroupId, @Status, @Gender, @AdmissionFrom, @AdmissionTo", parameters)
            .ToListAsync(ct);

        int totalRecords = items.FirstOrDefault()?.TotalRecords ?? 0;
        return (items, totalRecords);
    }

    public async Task<(List<EmployeeIdCardListDto> Items, int TotalRecords)> GetEmployeeIdCardListAsync(
        int page, int pageSize, string? search,
        int? departmentId, int? designationId,
        string? status, string? employmentType,
        DateTime? joiningFrom, DateTime? joiningTo,
        CancellationToken ct)
    {
        var parameters = new[]
        {
            new SqlParameter("@PageNumber", page),
            new SqlParameter("@PageSize", pageSize),
            new SqlParameter("@SearchTerm", (object?)search ?? DBNull.Value),
            new SqlParameter("@DepartmentId", departmentId ?? 0),
            new SqlParameter("@DesignationId", designationId ?? 0),
            new SqlParameter("@Status", (object?)status ?? DBNull.Value),
            new SqlParameter("@EmploymentType", (object?)employmentType ?? DBNull.Value),
            new SqlParameter("@JoiningFrom", (object?)joiningFrom ?? DBNull.Value),
            new SqlParameter("@JoiningTo", (object?)joiningTo ?? DBNull.Value)
        };

        var items = await _db.Set<EmployeeIdCardListDto>()
            .FromSqlRaw("EXEC sp_GetEmployeeIdCardList @PageNumber, @PageSize, @SearchTerm, @DepartmentId, @DesignationId, @Status, @EmploymentType, @JoiningFrom, @JoiningTo", parameters)
            .ToListAsync(ct);

        int totalRecords = items.FirstOrDefault()?.TotalRecords ?? 0;
        return (items, totalRecords);
    }

    public async Task<List<StudentIdCardBulkDto>> GetStudentIdCardBulkDataAsync(string ids, CancellationToken ct)
    {
        var parameter = new SqlParameter("@Ids", ids);
        return await _db.Set<StudentIdCardBulkDto>()
            .FromSqlRaw("EXEC sp_GetStudentIdCardBulkData @Ids", parameter)
            .ToListAsync(ct);
    }

    public async Task<List<EmployeeIdCardBulkDto>> GetEmployeeIdCardBulkDataAsync(string ids, CancellationToken ct)
    {
        var parameter = new SqlParameter("@Ids", ids);
        return await _db.Set<EmployeeIdCardBulkDto>()
            .FromSqlRaw("EXEC sp_GetEmployeeIdCardBulkData @Ids", parameter)
            .ToListAsync(ct);
    }
}
