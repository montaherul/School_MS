using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Result;

public class SubjectComponentService : ISubjectComponentService
{
    private readonly IUnitOfWork _uow;

    public SubjectComponentService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<List<SubjectComponent>> GetComponentsByClassSubjectAsync(int classSubjectId)
    {
        return await _uow.Repository<SubjectComponent>()
            .Query()
            .Where(c => c.ClassSubjectId == classSubjectId && !c.IsDeleted && c.IsActive)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();
    }

    public async Task<SubjectComponent?> CreateComponentAsync(SubjectComponent component)
    {
        await _uow.Repository<SubjectComponent>().AddAsync(component);
        await _uow.SaveChangesAsync();
        return component;
    }

    public async Task<SubjectComponent?> UpdateComponentAsync(SubjectComponent component)
    {
        _uow.Repository<SubjectComponent>().Update(component);
        await _uow.SaveChangesAsync();
        return component;
    }

    public async Task<bool> DeleteComponentAsync(int componentId)
    {
        var component = await _uow.Repository<SubjectComponent>().GetByIdAsync(componentId);
        if (component == null) return false;
        component.IsDeleted = true;
        _uow.Repository<SubjectComponent>().Update(component);
        await _uow.SaveChangesAsync();
        return true;
    }

    public async Task<List<SubjectComponent>> GetComponentsForSubjectAsync(int subjectId, int classId, int? groupId = null)
    {
        var query = _uow.Repository<ClassSubject>().Query()
            .Include(cs => cs.SubjectComponents)
            .Where(cs => cs.SubjectId == subjectId && cs.SchoolClassId == classId && !cs.IsDeleted && cs.IsActive);

        if (groupId.HasValue)
            query = query.Where(cs => cs.StudentGroupId == groupId);

        var classSubject = await query.FirstOrDefaultAsync();
        if (classSubject == null) return new List<SubjectComponent>();

        return classSubject.SubjectComponents
            .Where(c => !c.IsDeleted && c.IsActive)
            .OrderBy(c => c.DisplayOrder)
            .ToList();
    }
}
