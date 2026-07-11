using System.Data;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Repositories.Interfaces.Academic;

namespace SchoolManagementSystem.Repositories.Implementations.Academic;

public class AcademicYearRepository : BaseRepository<AcademicYear>, IAcademicYearRepository
{
    public AcademicYearRepository(SchoolDbContext db) : base(db) { }

    public async Task<List<AcademicYearSpResult>> GetListSpAsync(int pageNumber, int pageSize, string? searchTerm)
    {
        return await ExecuteStoredProcAsync<AcademicYearSpResult>(
            "sp_GetAcademicYearList", pageNumber, pageSize, searchTerm ?? (object)DBNull.Value);
    }
}

public class SchoolClassRepository : BaseRepository<SchoolClass>, ISchoolClassRepository
{
    public SchoolClassRepository(SchoolDbContext db) : base(db) { }

    public async Task<List<ClassListSpResult>> GetListSpAsync(int pageNumber, int pageSize, string? searchTerm)
    {
        return await ExecuteStoredProcAsync<ClassListSpResult>(
            "sp_GetClassList", pageNumber, pageSize, searchTerm ?? (object)DBNull.Value);
    }
}

public class SectionRepository : BaseRepository<Section>, ISectionRepository
{
    public SectionRepository(SchoolDbContext db) : base(db) { }

    public async Task<List<SectionListSpResult>> GetListSpAsync(int pageNumber, int pageSize, string? searchTerm)
    {
        return await ExecuteStoredProcAsync<SectionListSpResult>(
            "sp_GetSectionList", pageNumber, pageSize, searchTerm ?? (object)DBNull.Value);
    }
}

public class SubjectRepository : BaseRepository<Subject>, ISubjectRepository
{
    public SubjectRepository(SchoolDbContext db) : base(db) { }

    public async Task<List<SubjectListSpResult>> GetListSpAsync(int pageNumber, int pageSize, string? searchTerm)
    {
        return await ExecuteStoredProcAsync<SubjectListSpResult>(
            "sp_GetSubjectList", pageNumber, pageSize, searchTerm ?? (object)DBNull.Value);
    }
}
