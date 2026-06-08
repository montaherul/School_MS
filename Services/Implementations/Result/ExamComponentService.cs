using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Exam;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Result;

public class ExamComponentService : IExamComponentService
{
    private readonly IUnitOfWork _uow;

    public ExamComponentService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<List<ExamComponentListDto>> GetAllAsync(bool includeInactive = false)
    {
        var query = _uow.Repository<ExamComponent>().Query()
            .Where(e => !e.IsDeleted);

        if (!includeInactive)
            query = query.Where(e => e.IsActive);

        return await query
            .OrderBy(e => e.DisplayOrder)
            .ThenBy(e => e.Name)
            .Select(e => new ExamComponentListDto
            {
                Id = e.Id,
                Name = e.Name,
                Code = e.Code,
                Description = e.Description,
                DisplayOrder = e.DisplayOrder,
                DefaultFullMarks = e.DefaultFullMarks,
                DefaultPassMarks = e.DefaultPassMarks,
                IsPractical = e.IsPractical,
                IsOptional = e.IsOptional,
                IsActive = e.IsActive,
                CreatedAt = e.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<ExamComponentListDto?> GetByIdAsync(int id)
    {
        var entity = await _uow.Repository<ExamComponent>().Query()
            .Where(e => e.Id == id && !e.IsDeleted)
            .FirstOrDefaultAsync();

        if (entity == null) return null;

        return MapToDto(entity);
    }

    public async Task<ExamComponentListDto> CreateAsync(ExamComponentUpsertDto dto, string createdBy)
    {
        var entity = new ExamComponent
        {
            Name = dto.Name,
            Code = dto.Code.ToUpperInvariant(),
            Description = dto.Description,
            DisplayOrder = dto.DisplayOrder,
            DefaultFullMarks = dto.DefaultFullMarks,
            DefaultPassMarks = dto.DefaultPassMarks,
            IsPractical = dto.IsPractical,
            IsOptional = dto.IsOptional,
            IsActive = dto.IsActive,
            CreatedBy = createdBy
        };

        await _uow.Repository<ExamComponent>().AddAsync(entity);
        await _uow.SaveChangesAsync();

        return MapToDto(entity);
    }

    public async Task<ExamComponentListDto?> UpdateAsync(int id, ExamComponentUpsertDto dto, string updatedBy)
    {
        var entity = await _uow.Repository<ExamComponent>().GetByIdAsync(id);
        if (entity == null || entity.IsDeleted) return null;

        entity.Name = dto.Name;
        entity.Code = dto.Code.ToUpperInvariant();
        entity.Description = dto.Description;
        entity.DisplayOrder = dto.DisplayOrder;
        entity.DefaultFullMarks = dto.DefaultFullMarks;
        entity.DefaultPassMarks = dto.DefaultPassMarks;
        entity.IsPractical = dto.IsPractical;
        entity.IsOptional = dto.IsOptional;
        entity.IsActive = dto.IsActive;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;

        _uow.Repository<ExamComponent>().Update(entity);
        await _uow.SaveChangesAsync();

        return MapToDto(entity);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _uow.Repository<ExamComponent>().GetByIdAsync(id);
        if (entity == null || entity.IsDeleted) return false;

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        _uow.Repository<ExamComponent>().Update(entity);
        await _uow.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleActiveAsync(int id)
    {
        var entity = await _uow.Repository<ExamComponent>().GetByIdAsync(id);
        if (entity == null || entity.IsDeleted) return false;

        entity.IsActive = !entity.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;
        _uow.Repository<ExamComponent>().Update(entity);
        await _uow.SaveChangesAsync();
        return true;
    }

    private static ExamComponentListDto MapToDto(ExamComponent entity)
    {
        return new ExamComponentListDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Code = entity.Code,
            Description = entity.Description,
            DisplayOrder = entity.DisplayOrder,
            DefaultFullMarks = entity.DefaultFullMarks,
            DefaultPassMarks = entity.DefaultPassMarks,
            IsPractical = entity.IsPractical,
            IsOptional = entity.IsOptional,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt
        };
    }
}
