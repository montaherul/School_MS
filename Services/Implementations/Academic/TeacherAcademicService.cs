using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Academic;

public class TeacherAcademicService : ITeacherAcademicService
{
    private readonly SchoolDbContext _db;
    private readonly IUnitOfWork _uow;

    public TeacherAcademicService(SchoolDbContext db, IUnitOfWork uow)
    {
        _db = db;
        _uow = uow;
    }

    public async Task<IEnumerable<TeacherSubjectAssignmentDto>> GetAssignmentsByTeacherAsync(long employeeId, CancellationToken ct = default)
    {
        return await _db.EmployeeSubjectAssignments
            .Include(a => a.Subject)
            .Include(a => a.Class)
            .Include(a => a.Section)
            .Where(a => a.EmployeeId == employeeId && !a.IsDeleted)
            .Select(a => new TeacherSubjectAssignmentDto
            {
                Id = a.Id,
                EmployeeId = a.EmployeeId,
                SubjectId = a.SubjectId,
                SubjectName = a.Subject.Name,
                ClassId = a.ClassId,
                ClassName = a.Class.Name,
                SectionId = a.SectionId,
                SectionName = a.Section.Name,
                IsClassTeacher = a.IsClassTeacher
            })
            .ToListAsync(ct);
    }

    public async Task AssignSubjectAsync(TeacherSubjectAssignmentDto dto, string createdBy, CancellationToken ct = default)
    {
        var assignment = new EmployeeSubjectAssignment
        {
            EmployeeId = dto.EmployeeId,
            SubjectId = dto.SubjectId,
            ClassId = dto.ClassId,
            SectionId = dto.SectionId,
            AcademicYearId = 1, // Defaulting to 1 for now, should be dynamic
            IsClassTeacher = dto.IsClassTeacher,
            CreatedBy = createdBy
        };

        await _db.EmployeeSubjectAssignments.AddAsync(assignment, ct);
        await _uow.SaveChangesAsync(ct);
    }

    public async Task<TeacherWorkloadDto> GetWorkloadAsync(long employeeId, CancellationToken ct = default)
    {
        var assignments = await _db.EmployeeSubjectAssignments
            .Where(a => a.EmployeeId == employeeId && !a.IsDeleted)
            .ToListAsync(ct);

        var routine = await _db.ClassRoutines
            .Where(r => r.EmployeeId == employeeId && !r.IsDeleted)
            .ToListAsync(ct);

        return new TeacherWorkloadDto
        {
            EmployeeId = employeeId,
            TotalSubjects = assignments.Select(a => a.SubjectId).Distinct().Count(),
            TotalClasses = assignments.Select(a => a.ClassId).Distinct().Count(),
            WeeklyPeriods = routine.Count,
            PendingMarkEntries = 5 // Placeholder logic
        };
    }
}
