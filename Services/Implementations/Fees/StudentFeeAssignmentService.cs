using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Services.Interfaces.Fees;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Repositories.Interfaces.Fees;

namespace SchoolManagementSystem.Services.Implementations.Fees;

public class StudentFeeAssignmentService : IStudentFeeAssignmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStudentFeeAssignmentRepository _repository;

    public StudentFeeAssignmentService(IUnitOfWork unitOfWork, IStudentFeeAssignmentRepository repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<PagedResult<StudentFeeAssignmentListItemDto>> GetPagedAsync(int page, int pageSize, string? search, int? studentId = null, int? feeStructureId = null, CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 5, 100);
        var (items, totalCount) = await _repository.GetListByStoredProcedureAsync(page, pageSize, search, studentId, feeStructureId, cancellationToken);
        return new PagedResult<StudentFeeAssignmentListItemDto> { Items = items, Page = page, PageSize = pageSize, TotalItems = totalCount };
    }

    public async Task<StudentFeeAssignmentUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<StudentFeeAssignment>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        if (entity is null) return null;
        return new StudentFeeAssignmentUpsertDto
        { Id = entity.Id, StudentId = entity.StudentId, FeeStructureId = entity.FeeStructureId, AcademicYearId = entity.AcademicYearId, CustomAmount = entity.CustomAmount, IsActive = entity.IsActive, ValidFrom = entity.ValidFrom, ValidTo = entity.ValidTo };
    }

    public async Task<int> CreateAsync(StudentFeeAssignmentUpsertDto dto, string createdBy, CancellationToken cancellationToken = default)
    {
        var entity = new StudentFeeAssignment { CreatedBy = createdBy, StudentId = dto.StudentId, FeeStructureId = dto.FeeStructureId, AcademicYearId = dto.AcademicYearId, CustomAmount = dto.CustomAmount, IsActive = dto.IsActive, ValidFrom = dto.ValidFrom, ValidTo = dto.ValidTo };
        await _unitOfWork.Repository<StudentFeeAssignment>().AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    public async Task UpdateAsync(StudentFeeAssignmentUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<StudentFeeAssignment>().FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("StudentFeeAssignment not found.");
        entity.StudentId = dto.StudentId; entity.FeeStructureId = dto.FeeStructureId; entity.AcademicYearId = dto.AcademicYearId;
        entity.CustomAmount = dto.CustomAmount; entity.IsActive = dto.IsActive; entity.ValidFrom = dto.ValidFrom; entity.ValidTo = dto.ValidTo;
        entity.UpdatedBy = updatedBy; entity.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<StudentFeeAssignment>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<StudentFeeAssignment>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("StudentFeeAssignment not found.");
        entity.IsDeleted = true; entity.UpdatedBy = updatedBy; entity.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<StudentFeeAssignment>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RestoreAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<StudentFeeAssignment>().FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("StudentFeeAssignment not found or not deleted.");
        entity.IsDeleted = false;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<StudentFeeAssignment>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
