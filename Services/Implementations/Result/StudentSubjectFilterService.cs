using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.Entities.Academic;
using StudentModel = SchoolManagementSystem.Models.Entities.Student.Student;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Result;

public class StudentSubjectFilterService : IStudentSubjectFilterService
{
    private readonly IUnitOfWork _uow;

    public StudentSubjectFilterService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<HashSet<int>> GetValidSubjectIdsForStudentAsync(StudentModel student, CancellationToken ct = default)
    {
        var validIds = new HashSet<int>();

        if (student == null) return validIds;

        var classSubjects = await _uow.Repository<ClassSubject>().Query()
            .AsNoTracking()
            .Include(cs => cs.ClassSubjectGroups)
            .Where(cs => cs.SchoolClassId == student.ClassId && !cs.IsDeleted && cs.IsActive)
            .ToListAsync(ct);

        foreach (var cs in classSubjects)
        {
            if (cs.IsReligionSubject)
            {
                if (student.AssignedReligionSubjectId.HasValue && cs.SubjectId == student.AssignedReligionSubjectId.Value)
                    validIds.Add(cs.SubjectId);
                continue;
            }

            // Group-specific subject: include only if student's group is linked
            var link = cs.ClassSubjectGroups?.FirstOrDefault(csg => !csg.IsDeleted);
            if (link != null)
            {
                if (student.StudentGroupId.HasValue && link.StudentGroupId == student.StudentGroupId.Value)
                    validIds.Add(cs.SubjectId);
                continue;
            }

            // General subject (no group links): include for all students
            validIds.Add(cs.SubjectId);
        }

        if (student.OptionalSubjectId.HasValue)
            validIds.Add(student.OptionalSubjectId.Value);

        return validIds;
    }
}
