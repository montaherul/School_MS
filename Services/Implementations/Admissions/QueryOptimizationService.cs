using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.Entities.Admission;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.Admission;

namespace SchoolManagementSystem.Services.Implementations.Admissions;

/// <summary>
/// Optimized query service using compiled queries, projection, and AsNoTracking
/// </summary>
public static class AdmissionQueryOptimizer
{
    // Compiled query for getting admission by ID with projection
    private static readonly Func<SchoolManagementSystem.Data.SchoolDbContext, int, Task<AdmissionApplication?>>
        GetByIdCompiled = EF.CompileAsyncQuery(
            (SchoolManagementSystem.Data.SchoolDbContext ctx, int id) =>
                ctx.Admissions.AsNoTracking()
                    .Include(a => a.Documents.Where(d => !d.IsDeleted))
                    .FirstOrDefault(a => a.Id == id && !a.IsDeleted));

    // Compiled query for count by status
    private static readonly Func<SchoolManagementSystem.Data.SchoolDbContext, AdmissionStatus, int, Task<int>>
        CountByStatusCompiled = EF.CompileAsyncQuery(
            (SchoolManagementSystem.Data.SchoolDbContext ctx, AdmissionStatus status, int year) =>
                ctx.Admissions.Count(a => a.Status == status && !a.IsDeleted && a.CreatedAt.Year == year));

    public static async Task<AdmissionApplication?> GetByIdOptimizedAsync(
        IAdmissionRepository repository, int id, CancellationToken ct = default)
    {
        return await repository.Query().AsNoTracking()
            .Include(a => a.Documents.Where(d => !d.IsDeleted))
            .AsSplitQuery()
            .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted, ct);
    }

    public static IQueryable<object> GetProjectedListQuery(IAdmissionRepository repository)
    {
        return repository.Query().AsNoTracking()
            .Where(a => !a.IsDeleted)
            .Select(a => new
            {
                a.Id,
                a.ApplicationNo,
                a.ApplicantName,
                a.ApplicantNameBangla,
                a.DateOfBirth,
                a.Gender,
                a.AppliedClassId,
                a.Status,
                a.FatherName,
                a.MotherName,
                a.ApplicantMobileNumber,
                a.ApplicantEmail,
                a.Religion,
                a.AdmissionFee,
                a.AdmissionFeePaid,
                a.ProfilePicturePath,
                a.CreatedAt,
                a.CreatedBy
            });
    }

    public static async Task<int> GetStatusCountAsync(
        IAdmissionRepository repository, AdmissionStatus status, int? classId = null, CancellationToken ct = default)
    {
        var query = repository.Query().AsNoTracking()
            .Where(a => !a.IsDeleted && a.Status == status);

        if (classId.HasValue && classId.Value > 0)
            query = query.Where(a => a.AppliedClassId == classId.Value);

        return await query.CountAsync(ct);
    }

    public static async Task<Dictionary<AdmissionStatus, int>> GetAllStatusCountsAsync(
        IAdmissionRepository repository, int? classId = null, CancellationToken ct = default)
    {
        var query = repository.Query().AsNoTracking().Where(a => !a.IsDeleted);

        if (classId.HasValue && classId.Value > 0)
            query = query.Where(a => a.AppliedClassId == classId.Value);

        return await query
            .GroupBy(a => a.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.Status, g => g.Count, ct);
    }
}
