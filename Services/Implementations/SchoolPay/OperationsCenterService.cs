using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Models.DTOs.SchoolPay;
using SchoolManagementSystem.Repositories.Interfaces.SchoolPay;
using SchoolManagementSystem.Services.Interfaces.SchoolPay;

namespace SchoolManagementSystem.Services.Implementations.SchoolPay;

public class OperationsCenterService : IOperationsCenterService
{
    private readonly ISchoolPayRepository _repo;
    private readonly ILogger<OperationsCenterService> _logger;

    public OperationsCenterService(ISchoolPayRepository repo, ILogger<OperationsCenterService> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<SchoolPayOperationsDto> GetOperationsDataAsync(CancellationToken ct = default)
        => await _repo.GetOperationsDataAsync(ct);
}
