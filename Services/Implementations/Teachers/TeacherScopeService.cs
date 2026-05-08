using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Services.Interfaces.Teachers;

namespace SchoolManagementSystem.Services.Implementations.Teachers;

public class TeacherScopeService : ITeacherScopeService
{
    private readonly SchoolDbContext _db;

    public TeacherScopeService(SchoolDbContext db)
    {
        _db = db;
    }

    private async Task<int?> GetTeacherIdByUserIdAsync(int userId, CancellationToken ct)
    {
        return await _db.Teachers
            .AsNoTracking()
            .Where(t => t.UserId == userId && !t.IsDeleted)
            .Select(t => (int?)t.Id)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<bool> HasClassAccessAsync(int userId, int classId, int sectionId, CancellationToken ct = default)
    {
        var teacherId = await GetTeacherIdByUserIdAsync(userId, ct);
        if (teacherId == null) return false;

        return await _db.TeacherClassAssignments
            .AnyAsync(a => a.TeacherId == teacherId && a.ClassId == classId && a.SectionId == sectionId && !a.IsDeleted, ct);
    }

    public async Task<bool> HasSubjectAccessAsync(int userId, int subjectId, int classId, int sectionId, CancellationToken ct = default)
    {
        var teacherId = await GetTeacherIdByUserIdAsync(userId, ct);
        if (teacherId == null) return false;

        return await _db.TeacherSubjectAssignments
            .AnyAsync(a => a.TeacherId == teacherId && a.SubjectId == subjectId && a.ClassId == classId && a.SectionId == sectionId && !a.IsDeleted, ct);
    }

    public async Task<bool> HasStudentAccessAsync(int userId, int studentId, CancellationToken ct = default)
    {
        var teacherId = await GetTeacherIdByUserIdAsync(userId, ct);
        if (teacherId == null) return false;

        // A teacher has access to a student if they are assigned to the student's class/section
        var student = await _db.Students
            .AsNoTracking()
            .Where(s => s.Id == studentId && !s.IsDeleted)
            .Select(s => new { s.ClassId, s.SectionId })
            .FirstOrDefaultAsync(ct);

        if (student == null) return false;

        return await HasClassAccessAsync(userId, student.ClassId, student.SectionId, ct);
    }

    public async Task<IEnumerable<int>> GetAssignedClassIdsAsync(int userId, CancellationToken ct = default)
    {
        var teacherId = await GetTeacherIdByUserIdAsync(userId, ct);
        if (teacherId == null) return Enumerable.Empty<int>();

        return await _db.TeacherClassAssignments
            .Where(a => a.TeacherId == teacherId && !a.IsDeleted)
            .Select(a => a.ClassId)
            .Distinct()
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<int>> GetAssignedSectionIdsAsync(int userId, int classId, CancellationToken ct = default)
    {
        var teacherId = await GetTeacherIdByUserIdAsync(userId, ct);
        if (teacherId == null) return Enumerable.Empty<int>();

        return await _db.TeacherClassAssignments
            .Where(a => a.TeacherId == teacherId && a.ClassId == classId && !a.IsDeleted)
            .Select(a => a.SectionId)
            .Distinct()
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<int>> GetAssignedSubjectIdsAsync(int userId, int classId, int sectionId, CancellationToken ct = default)
    {
        var teacherId = await GetTeacherIdByUserIdAsync(userId, ct);
        if (teacherId == null) return Enumerable.Empty<int>();

        return await _db.TeacherSubjectAssignments
            .Where(a => a.TeacherId == teacherId && a.ClassId == classId && a.SectionId == sectionId && !a.IsDeleted)
            .Select(a => a.SubjectId)
            .Distinct()
            .ToListAsync(ct);
    }
}
