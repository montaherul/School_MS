using SchoolManagementSystem.Models.DTOs.SchoolPay;

namespace SchoolManagementSystem.Services.Interfaces.SchoolPay;

public interface IDeadLetterQueueService
{
    Task<List<SchoolPayDeadLetterDto>> GetAllAsync(CancellationToken ct = default);
    Task<SchoolPayDeadLetterDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task ReprocessAsync(int id, CancellationToken ct = default);
    Task IgnoreAsync(int id, CancellationToken ct = default);
    Task<int> GetCountAsync(CancellationToken ct = default);
}
