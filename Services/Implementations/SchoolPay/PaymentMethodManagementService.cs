using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Models.DTOs.SchoolPay;
using SchoolManagementSystem.Models.Entities.SchoolPay;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.SchoolPay;
using SchoolManagementSystem.Services.Interfaces.SchoolPay;

namespace SchoolManagementSystem.Services.Implementations.SchoolPay;

public class PaymentMethodManagementService : IPaymentMethodManagementService
{
    private readonly ISchoolPayRepository _repository;
    private readonly ILogger<PaymentMethodManagementService> _logger;

    public PaymentMethodManagementService(
        ISchoolPayRepository repository,
        ILogger<PaymentMethodManagementService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<List<SchoolPayProviderMethodDto>> GetAllMethodsAsync(CancellationToken ct = default)
        => await _repository.GetAllPaymentMethodsAsync(ct);

    public async Task<SchoolPayProviderMethodDto?> GetMethodByIdAsync(int id, CancellationToken ct = default)
    {
        var methods = await _repository.GetAllPaymentMethodsAsync(ct);
        return methods.FirstOrDefault(m => m.Id == id);
    }

    public async Task<int> CreateMethodAsync(SchoolPayMethodUpsertDto dto, string createdBy, CancellationToken ct = default)
    {
        var method = new SchoolManagementSystem.Models.Entities.SchoolPay.PaymentMethod
        {
            Code = dto.Code,
            Name = dto.Name,
            LogoUrl = dto.LogoUrl,
            PaymentProviderId = dto.PaymentProviderId,
            DisplayOrder = dto.DisplayOrder,
            IsDefault = dto.IsDefault,
            IsRecommended = dto.IsRecommended,
            IsPopular = dto.IsPopular,
            PopularityRank = dto.PopularityRank,
            BackgroundColor = dto.BackgroundColor,
            TextColor = dto.TextColor,
            Icon = dto.Icon,
            CssClass = dto.CssClass,
            IsActive = true,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };
        return await _repository.CreatePaymentMethodAsync(method, ct);
    }

    public async Task<bool> UpdateMethodAsync(int id, SchoolPayMethodUpsertDto dto, string updatedBy, CancellationToken ct = default)
    {
        var method = await _repository.GetPaymentMethodEntityByIdAsync(id, ct);
        if (method == null) return false;

        method.Code = dto.Code;
        method.Name = dto.Name;
        method.LogoUrl = dto.LogoUrl;
        method.PaymentProviderId = dto.PaymentProviderId;
        method.DisplayOrder = dto.DisplayOrder;
        method.IsDefault = dto.IsDefault;
        method.IsRecommended = dto.IsRecommended;
        method.IsPopular = dto.IsPopular;
        method.PopularityRank = dto.PopularityRank;
        method.BackgroundColor = dto.BackgroundColor;
        method.TextColor = dto.TextColor;
        method.Icon = dto.Icon;
        method.CssClass = dto.CssClass;
        method.UpdatedBy = updatedBy;
        method.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdatePaymentMethodAsync(method, ct);
        return true;
    }

    public async Task<bool> ToggleMethodActiveAsync(int id, bool isActive, string updatedBy, CancellationToken ct = default)
    {
        var method = await _repository.GetPaymentMethodEntityByIdAsync(id, ct);
        if (method == null) return false;
        method.IsActive = isActive;
        method.UpdatedBy = updatedBy;
        method.UpdatedAt = DateTime.UtcNow;
        await _repository.UpdatePaymentMethodAsync(method, ct);
        return true;
    }

    public async Task<bool> UpdateMethodOrderAsync(int id, int displayOrder, CancellationToken ct = default)
        => await _repository.UpdatePaymentMethodOrderAsync(id, displayOrder, ct);

    public async Task<bool> DeleteMethodAsync(int id, CancellationToken ct = default)
    {
        var method = await _repository.GetPaymentMethodEntityByIdAsync(id, ct);
        if (method == null) return false;
        method.IsDeleted = true;
        method.UpdatedAt = DateTime.UtcNow;
        await _repository.UpdatePaymentMethodAsync(method, ct);
        return true;
    }

    public async Task<List<SchoolPayProviderMethodDto>> GetActiveMethodsForCheckoutAsync(CancellationToken ct = default)
    {
        var methods = await _repository.GetAllPaymentMethodsAsync(ct);
        return methods.Where(m => m.IsActive).OrderBy(m => m.DisplayOrder).ToList();
    }
}
