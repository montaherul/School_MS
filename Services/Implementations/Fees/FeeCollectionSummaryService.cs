using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Fees;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Repositories.Interfaces.Fees;

namespace SchoolManagementSystem.Services.Implementations.Fees;

public class FeeCollectionSummaryService : IFeeCollectionSummaryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFeeCollectionSummaryRepository _repository;

    public FeeCollectionSummaryService(IUnitOfWork unitOfWork, IFeeCollectionSummaryRepository repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<PagedResult<FeeCollectionSummaryListItemDto>> GetPagedAsync(int page, int pageSize, string? search, DateOnly? fromDate = null, DateOnly? toDate = null, CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 5, 100);
        var (items, totalCount) = await _repository.GetListByStoredProcedureAsync(page, pageSize, search, fromDate, toDate, cancellationToken);
        return new PagedResult<FeeCollectionSummaryListItemDto> { Items = items, Page = page, PageSize = pageSize, TotalItems = totalCount };
    }

    public async Task<FeeCollectionSummaryUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<FeeCollectionSummary>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        if (entity is null) return null;
        return new FeeCollectionSummaryUpsertDto { Id = entity.Id, CollectionDate = entity.CollectionDate, TotalCollected = entity.TotalCollected, TotalDiscounted = entity.TotalDiscounted, TotalRefunded = entity.TotalRefunded, TotalTransactions = entity.TotalTransactions, PaymentMethod = (int?)entity.PaymentMethod, IsDailySummary = entity.IsDailySummary };
    }

    public async Task<int> CreateAsync(FeeCollectionSummaryUpsertDto dto, string createdBy, CancellationToken cancellationToken = default)
    {
        var entity = new FeeCollectionSummary { CreatedBy = createdBy, CollectionDate = dto.CollectionDate, TotalCollected = dto.TotalCollected, TotalDiscounted = dto.TotalDiscounted, TotalRefunded = dto.TotalRefunded, TotalTransactions = dto.TotalTransactions, PaymentMethod = (PaymentMethod?)dto.PaymentMethod, IsDailySummary = dto.IsDailySummary };
        await _unitOfWork.Repository<FeeCollectionSummary>().AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    public async Task UpdateAsync(FeeCollectionSummaryUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<FeeCollectionSummary>().FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("FeeCollectionSummary not found.");
        entity.CollectionDate = dto.CollectionDate; entity.TotalCollected = dto.TotalCollected; entity.TotalDiscounted = dto.TotalDiscounted;
        entity.TotalRefunded = dto.TotalRefunded; entity.TotalTransactions = dto.TotalTransactions; entity.PaymentMethod = (PaymentMethod?)dto.PaymentMethod; entity.IsDailySummary = dto.IsDailySummary;
        entity.UpdatedBy = updatedBy; entity.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<FeeCollectionSummary>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<FeeCollectionSummary>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("FeeCollectionSummary not found.");
        entity.IsDeleted = true; entity.UpdatedBy = updatedBy; entity.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<FeeCollectionSummary>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
