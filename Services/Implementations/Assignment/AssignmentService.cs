using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.Entities.Assignment;
using SchoolManagementSystem.Models.Entities.Teachers;
using SchoolManagementSystem.Models.Entities.Student;
using SchoolManagementSystem.Services.Implementations.Base;
using SchoolManagementSystem.Services.Interfaces.Assignment;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Assignment;

public class AssignmentService : BaseService<AssignmentTask>, IAssignmentService
{
    private readonly IUnitOfWork _unitOfWork;

    public AssignmentService(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IQueryable<AssignmentTask>> ApplySecurityFiltersAsync(IQueryable<AssignmentTask> query, int userId, bool isStudent, bool isTeacher, bool isAdmin, CancellationToken ct = default)
    {
        if (isStudent)
        {
            var student = await _unitOfWork.Repository<Student>().Query().AsNoTracking().FirstOrDefaultAsync(s => s.UserId == userId && !s.IsDeleted, ct);
            if (student != null) return query.Where(a => a.SchoolClassId == student.ClassId && a.SectionId == student.SectionId);
            return query.Where(a => false);
        }

        if (isTeacher && !isAdmin)
        {
<<<<<<< HEAD
            var teacher = await _unitOfWork.Repository<Teacher>().Query().AsNoTracking().FirstOrDefaultAsync(t => t.UserId == userId && !t.IsDeleted, ct);
=======
            var teacher = await _unitOfWork.Repository<Teacher>().Query().AsNoTracking().FirstOrDefaultAsync(t => t.Employee!.UserId == userId && !t.IsDeleted, ct);
>>>>>>> d8b24e6 (attendece and website curtomize)
            if (teacher != null)
            {
                var assignedClassIds = await _unitOfWork.Repository<TeacherClassAssignment>().Query()
                    .Where(a => a.TeacherId == teacher.Id && !a.IsDeleted)
                    .Select(a => a.ClassId)
                    .ToListAsync(ct);
                
                return query.Where(a => assignedClassIds.Contains(a.SchoolClassId));
            }
            return query.Where(a => false);
        }

        return query;
    }
}

