using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Student;
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

    public async Task<HashSet<int>> GetValidSubjectIdsForStudentAsync(Student student, CancellationToken ct = default)
    {
        var validIds = new HashSet<int>();

        if (student == null) return validIds;

        var classSubjects = await _uow.Repository<ClassSubject>().Query()
            .AsNoTracking()
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

            if (cs.IsGroupSubject)
            {
                if (cs.StudentGroupId.HasValue && student.StudentGroupId.HasValue &&
                    cs.StudentGroupId.Value == student.StudentGroupId.Value)
                    validIds.Add(cs.SubjectId);
                continue;
            }

            validIds.Add(cs.SubjectId);
        }

        if (student.OptionalSubjectId.HasValue)
            validIds.Add(student.OptionalSubjectId.Value);

        return validIds;
    }
}
