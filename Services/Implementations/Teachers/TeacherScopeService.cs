using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Services.Interfaces.Teachers;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Repositories.Interfaces.Teachers;
using SchoolManagementSystem.Repositories.Interfaces.Students;
using SchoolManagementSystem.Models.Entities.Teachers;
using SchoolManagementSystem.Models.Entities.Student;

namespace SchoolManagementSystem.Services.Implementations.Teachers;

public class TeacherScopeService : ITeacherScopeService
{
    private readonly IUnitOfWork _uow;
    private readonly ITeacherRepository _teacherRepository;
    private readonly ITeacherClassAssignmentRepository _classAssignmentRepository;
    private readonly ITeacherSubjectAssignmentRepository _subjectAssignmentRepository;
    private readonly IStudentRepository _studentRepository;

    public TeacherScopeService(
        IUnitOfWork uow,
        ITeacherRepository teacherRepository,
        ITeacherClassAssignmentRepository classAssignmentRepository,
        ITeacherSubjectAssignmentRepository subjectAssignmentRepository,
        IStudentRepository studentRepository)
    {
        _uow = uow;
        _teacherRepository = teacherRepository;
        _classAssignmentRepository = classAssignmentRepository;
        _subjectAssignmentRepository = subjectAssignmentRepository;
        _studentRepository = studentRepository;
    }

    private async Task<int?> GetTeacherIdByUserIdAsync(int userId, CancellationToken ct)
    {
        return await _teacherRepository.Query()
            .AsNoTracking()
            .Where(t => t.Employee.UserId == userId && !t.IsDeleted)
            .Select(t => (int?)t.Id)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<bool> HasClassAccessAsync(int userId, int classId, int sectionId, CancellationToken ct = default)
    {
        var teacherId = await GetTeacherIdByUserIdAsync(userId, ct);
        if (teacherId == null) return false;

        return await _classAssignmentRepository.Query()
            .AnyAsync(a => a.TeacherId == teacherId && a.ClassId == classId && a.SectionId == sectionId && !a.IsDeleted, ct);
    }

    public async Task<bool> HasSubjectAccessAsync(int userId, int subjectId, int classId, int sectionId, CancellationToken ct = default)
    {
        var teacherId = await GetTeacherIdByUserIdAsync(userId, ct);
        if (teacherId == null) return false;

        return await _subjectAssignmentRepository.Query()
            .AnyAsync(a => a.TeacherId == teacherId && a.SubjectId == subjectId && a.ClassId == classId && a.SectionId == sectionId && !a.IsDeleted, ct);
    }

    public async Task<bool> HasStudentAccessAsync(int userId, int studentId, CancellationToken ct = default)
    {
        var teacherId = await GetTeacherIdByUserIdAsync(userId, ct);
        if (teacherId == null) return false;

        var student = await _studentRepository.Query()
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


        return await _classAssignmentRepository.Query()
            .Where(a => a.TeacherId == teacherId && !a.IsDeleted)

        var activeYear = await _uow.Repository<AcademicYear>().Query()
            .FirstOrDefaultAsync(y => y.IsActive && !y.IsDeleted, ct);
            
        if (activeYear == null) return Enumerable.Empty<int>();

        return await _classAssignmentRepository.Query()
            .Where(a => a.TeacherId == teacherId && a.AcademicYearId == activeYear.Id && a.IsActive && !a.IsDeleted)

            .Select(a => a.ClassId)
            .Distinct()
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<int>> GetAssignedSectionIdsAsync(int userId, int classId, CancellationToken ct = default)
    {
        var teacherId = await GetTeacherIdByUserIdAsync(userId, ct);
        if (teacherId == null) return Enumerable.Empty<int>();


        return await _classAssignmentRepository.Query()
            .Where(a => a.TeacherId == teacherId && a.ClassId == classId && !a.IsDeleted)

        var activeYear = await _uow.Repository<AcademicYear>().Query()
            .FirstOrDefaultAsync(y => y.IsActive && !y.IsDeleted, ct);
            
        if (activeYear == null) return Enumerable.Empty<int>();

        return await _classAssignmentRepository.Query()
            .Where(a => a.TeacherId == teacherId && a.ClassId == classId && a.AcademicYearId == activeYear.Id && a.IsActive && !a.IsDeleted)

            .Select(a => a.SectionId)
            .Distinct()
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<int>> GetAssignedSubjectIdsAsync(int userId, int classId, int sectionId, CancellationToken ct = default)
    {
        var teacherId = await GetTeacherIdByUserIdAsync(userId, ct);
        if (teacherId == null) return Enumerable.Empty<int>();

        return await _subjectAssignmentRepository.Query()
            .Where(a => a.TeacherId == teacherId && a.ClassId == classId && a.SectionId == sectionId && !a.IsDeleted)
            .Select(a => a.SubjectId)
            .Distinct()
            .ToListAsync(ct);
    }
}

