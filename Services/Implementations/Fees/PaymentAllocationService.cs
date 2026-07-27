using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Services.Interfaces.Fees;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Repositories.Interfaces.Fees;

namespace SchoolManagementSystem.Services.Implementations.Fees;

public class PaymentAllocationService : IPaymentAllocationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaymentAllocationRepository _repository;

    public PaymentAllocationService(IUnitOfWork unitOfWork, IPaymentAllocationRepository repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<PagedResult<PaymentAllocationListItemDto>> GetPagedAsync(int page, int pageSize, string? search, int? paymentId, int? feeInvoiceId, CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 5, 100);
        var (items, totalCount) = await _repository.GetListByStoredProcedureAsync(page, pageSize, search, paymentId, feeInvoiceId, cancellationToken);
        return new PagedResult<PaymentAllocationListItemDto> { Items = items, Page = page, PageSize = pageSize, TotalItems = totalCount };
    }

    public async Task<PaymentAllocationUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<PaymentAllocation>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        if (entity is null) return null;
        return new PaymentAllocationUpsertDto { Id = entity.Id, PaymentId = entity.PaymentId, FeeInvoiceId = entity.FeeInvoiceId, AllocatedAmount = entity.AllocatedAmount, Remarks = entity.Remarks };
    }

    public async Task<int> CreateAsync(PaymentAllocationUpsertDto dto, string createdBy, CancellationToken cancellationToken = default)
    {
        var entity = new PaymentAllocation { CreatedBy = createdBy, PaymentId = dto.PaymentId, FeeInvoiceId = dto.FeeInvoiceId, AllocatedAmount = dto.AllocatedAmount, Remarks = dto.Remarks };
        await _unitOfWork.Repository<PaymentAllocation>().AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    public async Task UpdateAsync(PaymentAllocationUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<PaymentAllocation>().FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("PaymentAllocation not found.");
        entity.PaymentId = dto.PaymentId; entity.FeeInvoiceId = dto.FeeInvoiceId; entity.AllocatedAmount = dto.AllocatedAmount; entity.Remarks = dto.Remarks;
        entity.UpdatedBy = updatedBy; entity.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<PaymentAllocation>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<PaymentAllocation>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("PaymentAllocation not found.");
        entity.IsDeleted = true; entity.UpdatedBy = updatedBy; entity.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<PaymentAllocation>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RestoreAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<PaymentAllocation>().FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("PaymentAllocation not found or not deleted.");
        entity.IsDeleted = false;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<PaymentAllocation>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
