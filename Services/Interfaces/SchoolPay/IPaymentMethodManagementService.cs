using SchoolManagementSystem.Models.DTOs.SchoolPay;

namespace SchoolManagementSystem.Services.Interfaces.SchoolPay;

public interface IPaymentMethodManagementService
{
    Task<List<SchoolPayProviderMethodDto>> GetAllMethodsAsync(CancellationToken ct = default);
    Task<SchoolPayProviderMethodDto?> GetMethodByIdAsync(int id, CancellationToken ct = default);
    Task<int> CreateMethodAsync(SchoolPayMethodUpsertDto dto, string createdBy, CancellationToken ct = default);
    Task<bool> UpdateMethodAsync(int id, SchoolPayMethodUpsertDto dto, string updatedBy, CancellationToken ct = default);
    Task<bool> ToggleMethodActiveAsync(int id, bool isActive, string updatedBy, CancellationToken ct = default);
    Task<bool> UpdateMethodOrderAsync(int id, int displayOrder, CancellationToken ct = default);
    Task<bool> DeleteMethodAsync(int id, CancellationToken ct = default);
    Task<List<SchoolPayProviderMethodDto>> GetActiveMethodsForCheckoutAsync(CancellationToken ct = default);
}
