using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Exam;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Result;

public class SubjectMarkStructureService : ISubjectMarkStructureService
{
    private readonly IUnitOfWork _uow;

    public SubjectMarkStructureService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<List<SubjectMarkStructureDto>> GetByExamAsync(int examId)
    {
        return await _uow.Repository<SubjectMarkStructure>().Query()
            .Include(s => s.Component)
            .Include(s => s.Exam)
            .Include(s => s.Class)
            .Include(s => s.Subject)
            .Include(s => s.StudentGroup)
            .Where(s => s.ExamId == examId && !s.IsDeleted && s.IsActive)
            .OrderBy(s => s.DisplayOrder)
            .ThenBy(s => s.Component!.DisplayOrder)
            .Select(s => MapToDto(s))
            .ToListAsync();
    }

    public async Task<List<SubjectMarkStructureDto>> GetBySubjectAsync(int examId, int subjectId)
    {
        return await _uow.Repository<SubjectMarkStructure>().Query()
            .Include(s => s.Component)
            .Include(s => s.Exam)
            .Include(s => s.Class)
            .Include(s => s.Subject)
            .Include(s => s.StudentGroup)
            .Where(s => s.ExamId == examId && s.SubjectId == subjectId && !s.IsDeleted && s.IsActive)
            .OrderBy(s => s.DisplayOrder)
            .ThenBy(s => s.Component!.DisplayOrder)
            .Select(s => MapToDto(s))
            .ToListAsync();
    }

    public async Task<SubjectMarkStructureDto?> GetByIdAsync(int id)
    {
        var entity = await _uow.Repository<SubjectMarkStructure>().Query()
            .Include(s => s.Component)
            .Include(s => s.Exam)
            .Include(s => s.Class)
            .Include(s => s.Subject)
            .Include(s => s.StudentGroup)
            .Where(s => s.Id == id && !s.IsDeleted)
            .FirstOrDefaultAsync();

        return entity == null ? null : MapToDto(entity);
    }

    public async Task<SubjectMarkStructureDto> CreateAsync(SubjectMarkStructureUpsertDto dto, string createdBy)
    {
        Validate(dto);

        var entity = new SubjectMarkStructure
        {
            ComponentId = dto.ComponentId,
            ExamId = dto.ExamId,
            ClassId = dto.ClassId,
            SubjectId = dto.SubjectId,
            StudentGroupId = dto.StudentGroupId,
            FullMarks = dto.FullMarks,
            PassMarks = dto.PassMarks,
            DisplayOrder = dto.DisplayOrder,
            IsActive = dto.IsActive,
            CreatedBy = createdBy
        };

        await _uow.Repository<SubjectMarkStructure>().AddAsync(entity);
        await _uow.SaveChangesAsync();

        return (await GetByIdAsync(entity.Id))!;
    }

    public async Task<SubjectMarkStructureDto?> UpdateAsync(int id, SubjectMarkStructureUpsertDto dto, string updatedBy)
    {
        Validate(dto);

        var entity = await _uow.Repository<SubjectMarkStructure>().GetByIdAsync(id);
        if (entity == null || entity.IsDeleted) return null;

        entity.ComponentId = dto.ComponentId;
        entity.ExamId = dto.ExamId;
        entity.ClassId = dto.ClassId;
        entity.SubjectId = dto.SubjectId;
        entity.StudentGroupId = dto.StudentGroupId;
        entity.FullMarks = dto.FullMarks;
        entity.PassMarks = dto.PassMarks;
        entity.DisplayOrder = dto.DisplayOrder;
        entity.IsActive = dto.IsActive;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;

        _uow.Repository<SubjectMarkStructure>().Update(entity);
        await _uow.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _uow.Repository<SubjectMarkStructure>().GetByIdAsync(id);
        if (entity == null || entity.IsDeleted) return false;

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        _uow.Repository<SubjectMarkStructure>().Update(entity);
        await _uow.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SaveBulkAsync(int examId, int subjectId, List<SubjectMarkStructureUpsertDto> items, string updatedBy)
    {
        foreach (var item in items)
        {
            item.ExamId = examId;
            item.SubjectId = subjectId;
            Validate(item);
        }

        await _uow.ExecuteInTransactionAsync(async () =>
        {
            // Remove existing structures for this exam+subject
            var existing = await _uow.Repository<SubjectMarkStructure>().Query()
                .Where(s => s.ExamId == examId && s.SubjectId == subjectId && !s.IsDeleted)
                .ToListAsync();

            foreach (var e in existing)
            {
                e.IsDeleted = true;
                e.UpdatedBy = updatedBy;
                e.UpdatedAt = DateTime.UtcNow;
                _uow.Repository<SubjectMarkStructure>().Update(e);
            }

            // Add new structures
            foreach (var item in items)
            {
                var entity = new SubjectMarkStructure
                {
                    ComponentId = item.ComponentId,
                    ExamId = examId,
                    SubjectId = subjectId,
                    FullMarks = item.FullMarks,
                    PassMarks = item.PassMarks,
                    DisplayOrder = item.DisplayOrder,
                    IsActive = true,
                    CreatedBy = updatedBy
                };
                await _uow.Repository<SubjectMarkStructure>().AddAsync(entity);
            }
        });

        return true;
    }

    public async Task<List<ComponentColumnDto>> GetGridColumnsAsync(int examId, int subjectId, int? classId = null, int? studentGroupId = null)
    {
        var structures = await _uow.Repository<SubjectMarkStructure>().Query()
            .Include(s => s.Component)
            .Where(s => !s.IsDeleted && s.IsActive
                && s.Component!.IsActive
                && ((s.ExamId == examId && s.SubjectId == subjectId)
                    || (s.ExamId == null && s.SubjectId == subjectId)
                    || (s.ExamId == null && s.SubjectId == null && s.ClassId == classId)
                    || (s.ExamId == null && s.SubjectId == null && s.ClassId == null && s.StudentGroupId == studentGroupId)))
            .OrderBy(s => s.DisplayOrder)
            .ThenBy(s => s.Component!.DisplayOrder)
            .ToListAsync();

        if (structures.Count == 0)
        {
            var defaultComponents = await _uow.Repository<ExamComponent>().Query()
                .Where(e => !e.IsDeleted && e.IsActive)
                .OrderBy(e => e.DisplayOrder)
                .ToListAsync();

            return defaultComponents.Select(c => new ComponentColumnDto
            {
                ComponentId = c.Id,
                ComponentName = c.Name,
                ComponentCode = c.Code,
                FullMarks = c.DefaultFullMarks,
                PassMarks = c.DefaultPassMarks,
                DisplayOrder = c.DisplayOrder,
                FieldName = ResolveFieldName(c.Code)
            }).ToList();
        }

        return structures.Select(s => new ComponentColumnDto
        {
            ComponentId = s.Component!.Id,
            ComponentName = s.Component.Name,
            ComponentCode = s.Component.Code,
            FullMarks = s.FullMarks,
            PassMarks = s.PassMarks,
            DisplayOrder = s.DisplayOrder,
            FieldName = ResolveFieldName(s.Component.Code)
        }).ToList();
    }

    private static string ResolveFieldName(string componentCode)
    {
        var propName = ComponentFieldMapper.GetPropertyName(componentCode);
        if (propName != null)
            return JsonNamingPolicy.CamelCase.ConvertName(propName);

        return "cmp_" + componentCode;
    }

    private static void Validate(SubjectMarkStructureUpsertDto dto)
    {
        if (dto.FullMarks <= 0)
            throw new InvalidOperationException("Full marks must be greater than 0.");
        if (dto.PassMarks > dto.FullMarks)
            throw new InvalidOperationException("Pass marks cannot exceed full marks.");
        if (dto.FullMarks <= 0)
            throw new InvalidOperationException("Component marks must be greater than 0.");
    }

    private static SubjectMarkStructureDto MapToDto(SubjectMarkStructure entity)
    {
        return new SubjectMarkStructureDto
        {
            Id = entity.Id,
            ComponentId = entity.ComponentId,
            ComponentName = entity.Component?.Name ?? string.Empty,
            ComponentCode = entity.Component?.Code ?? string.Empty,
            ExamId = entity.ExamId,
            ExamName = entity.Exam?.Name ?? string.Empty,
            ClassId = entity.ClassId,
            ClassName = entity.Class?.Name ?? string.Empty,
            SubjectId = entity.SubjectId,
            SubjectName = entity.Subject?.Name ?? string.Empty,
            StudentGroupId = entity.StudentGroupId,
            StudentGroupName = entity.StudentGroup?.Name ?? string.Empty,
            FullMarks = entity.FullMarks,
            PassMarks = entity.PassMarks,
            DisplayOrder = entity.DisplayOrder,
            IsActive = entity.IsActive
        };
    }
}
