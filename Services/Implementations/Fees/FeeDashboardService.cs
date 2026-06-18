using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Services.Interfaces.Fees;
using SchoolManagementSystem.Repositories.Interfaces.Fees;

namespace SchoolManagementSystem.Services.Implementations.Fees;

public class FeeDashboardService : IFeeDashboardService
{
    private readonly IFeeDashboardRepository _repository;

    public FeeDashboardService(IFeeDashboardRepository repository)
    {
        _repository = repository;
    }

    public async Task<FeeDashboardDto> GetDashboardDataAsync(int? academicYearId = null, CancellationToken cancellationToken = default)
    {
        return await _repository.GetDashboardDataAsync(academicYearId, cancellationToken);
    }
}
