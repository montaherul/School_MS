using System.Data;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Repositories.Interfaces.Academic;

namespace SchoolManagementSystem.Repositories.Implementations.Academic;

public class ClassSubjectRepository : BaseRepository<ClassSubject>, IClassSubjectRepository
{
    public ClassSubjectRepository(SchoolDbContext db) : base(db) { }

    public async Task<List<ClassSubjectListItemDto>> GetPagedBySpAsync(int page, int pageSize, int? classId, string? groupName, string? search)
    {
        return await ExecuteStoredProcAsync<ClassSubjectListItemDto>(
            "sp_GetClassSubjectsPaged",
            page,
            pageSize,
            classId ?? (object)DBNull.Value,
            groupName ?? (object)DBNull.Value,
            search ?? (object)DBNull.Value);
    }
}
