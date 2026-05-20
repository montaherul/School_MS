using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Teacher;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Teachers;
using SchoolManagementSystem.Services.Interfaces.Teachers;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Teachers;

public class TeacherAssignmentService : ITeacherAssignmentService
{
    private readonly IUnitOfWork _unitOfWork;

    public TeacherAssignmentService(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<IEnumerable<TeacherClassAssignmentDto>> GetTeacherClassAssignmentsAsync(int teacherId, CancellationToken ct = default)
    {
        return await _unitOfWork.Repository<TeacherClassAssignment>().Query()
            .AsNoTracking()
            .Include(a => a.Class)
            .Include(a => a.Section)
            .Where(a => a.TeacherId == teacherId && !a.IsDeleted)
            .Select(a => new TeacherClassAssignmentDto
            {
                ClassId = a.ClassId,
                ClassName = a.Class != null ? a.Class.Name : string.Empty,
                SectionId = a.SectionId,
                SectionName = a.Section != null ? a.Section.Name : string.Empty
            })
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<TeacherSubjectAssignmentDto>> GetTeacherSubjectAssignmentsAsync(int teacherId, int classId, int sectionId, CancellationToken ct = default)
    {
        return await _unitOfWork.Repository<TeacherSubjectAssignment>().Query()
            .AsNoTracking()
            .Include(a => a.Subject)

            .Where(a => a.TeacherId == teacherId && a.ClassId == classId && a.SectionId == sectionId && !a.IsDeleted)

            .Where(a => a.TeacherId == teacherId 
                && (classId == 0 || a.ClassId == classId) 
                && (sectionId == 0 || a.SectionId == sectionId) 
                && !a.IsDeleted)

            .Select(a => new TeacherSubjectAssignmentDto
            {
                SubjectId = a.SubjectId,
                SubjectName = a.Subject != null ? a.Subject.Name : string.Empty,
                ClassId = a.ClassId,
                SectionId = a.SectionId
            })
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<SchoolClass>> GetClassesByTeacherIdAsync(int teacherId, CancellationToken ct = default)
    {
        return await _unitOfWork.Repository<TeacherClassAssignment>().Query()
            .AsNoTracking()
            .Include(a => a.Class)
            .Where(a => a.TeacherId == teacherId && !a.IsDeleted)
            .Select(a => a.Class!)
            .Distinct()
            .ToListAsync(ct);
    }

    public async Task<bool> AssignClassAsync(int teacherId, int classId, int sectionId, int academicYearId, string createdBy)
    {
        var repo = _unitOfWork.Repository<TeacherClassAssignment>();
        if (await repo.AnyAsync(a => a.TeacherId == teacherId && a.ClassId == classId && a.SectionId == sectionId && a.AcademicYearId == academicYearId && !a.IsDeleted)) return false;

        var assignment = new TeacherClassAssignment { TeacherId = teacherId, ClassId = classId, SectionId = sectionId, AcademicYearId = academicYearId, CreatedBy = createdBy, CreatedAt = DateTime.UtcNow };
        await repo.AddAsync(assignment);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AssignSubjectAsync(int teacherId, int subjectId, int classId, int sectionId, int academicYearId, string createdBy)
    {
        var repo = _unitOfWork.Repository<TeacherSubjectAssignment>();
        if (await repo.AnyAsync(a => a.TeacherId == teacherId && a.SubjectId == subjectId && a.ClassId == classId && a.SectionId == sectionId && a.AcademicYearId == academicYearId && !a.IsDeleted)) return false;

        var assignment = new TeacherSubjectAssignment { TeacherId = teacherId, SubjectId = subjectId, ClassId = classId, SectionId = sectionId, AcademicYearId = academicYearId, CreatedBy = createdBy, CreatedAt = DateTime.UtcNow };
        await repo.AddAsync(assignment);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task RemoveClassAssignmentAsync(int assignmentId)
    {
        var repo = _unitOfWork.Repository<TeacherClassAssignment>();
        var entity = await repo.FirstOrDefaultAsync(a => a.Id == assignmentId);
        if (entity != null) { entity.IsDeleted = true; await _unitOfWork.SaveChangesAsync(); }
    }

    public async Task RemoveSubjectAssignmentAsync(int assignmentId)
    {
        var repo = _unitOfWork.Repository<TeacherSubjectAssignment>();
        var entity = await repo.FirstOrDefaultAsync(a => a.Id == assignmentId);
        if (entity != null) { entity.IsDeleted = true; await _unitOfWork.SaveChangesAsync(); }
    }

    public async Task<Teacher?> GetTeacherWithAssignmentsAsync(int teacherId, CancellationToken ct = default)
    {
        return await _unitOfWork.Repository<Teacher>().Query()
            .Include(t => t.ClassAssignments).ThenInclude(a => a.Class)
            .Include(t => t.ClassAssignments).ThenInclude(a => a.Section)
            .Include(t => t.SubjectAssignments).ThenInclude(a => a.Subject)
            .Include(t => t.SubjectAssignments).ThenInclude(a => a.Class)
            .Include(t => t.SubjectAssignments).ThenInclude(a => a.Section)
            .FirstOrDefaultAsync(t => t.Id == teacherId && !t.IsDeleted, ct);
    }

    public async Task<IEnumerable<SchoolClass>> GetClassesAsync(CancellationToken ct = default)
    {
        return await _unitOfWork.Repository<SchoolClass>().Query().Where(x => !x.IsDeleted).ToListAsync(ct);
    }

    public async Task<IEnumerable<AcademicYear>> GetAcademicYearsAsync(CancellationToken ct = default)
    {
        return await _unitOfWork.Repository<AcademicYear>().Query().Where(x => !x.IsDeleted).ToListAsync(ct);
    }

    public async Task<IEnumerable<Subject>> GetSubjectsAsync(CancellationToken ct = default)
    {
        return await _unitOfWork.Repository<Subject>().Query().Where(x => !x.IsDeleted).OrderBy(x => x.Name).ToListAsync(ct);
    }
}

