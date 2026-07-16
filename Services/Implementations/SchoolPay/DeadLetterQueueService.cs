using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Models.DTOs.SchoolPay;
using SchoolManagementSystem.Repositories.Interfaces.SchoolPay;
using SchoolManagementSystem.Services.Interfaces.SchoolPay;

namespace SchoolManagementSystem.Services.Implementations.SchoolPay;

public class DeadLetterQueueService : IDeadLetterQueueService
{
    private readonly ISchoolPayRepository _repo;
    private readonly ILogger<DeadLetterQueueService> _logger;

    public DeadLetterQueueService(ISchoolPayRepository repo, ILogger<DeadLetterQueueService> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<List<SchoolPayDeadLetterDto>> GetAllAsync(CancellationToken ct = default)
        => await _repo.GetDeadLetterItemsAsync(ct);

    public async Task<SchoolPayDeadLetterDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var items = await _repo.GetDeadLetterItemsAsync(ct);
        return items.FirstOrDefault(i => i.Id == id);
    }

    public async Task ReprocessAsync(int id, CancellationToken ct = default)
    {
        _logger.LogInformation("Reprocessing dead letter webhook {Id}", id);
        await _repo.ReprocessDeadLetterAsync(id, ct);
    }

    public async Task IgnoreAsync(int id, CancellationToken ct = default)
    {
        _logger.LogInformation("Ignoring dead letter webhook {Id}", id);
        await _repo.IgnoreDeadLetterAsync(id, ct);
    }

    public async Task<int> GetCountAsync(CancellationToken ct = default)
    {
        var items = await _repo.GetDeadLetterItemsAsync(ct);
        return items.Count;
    }
}
