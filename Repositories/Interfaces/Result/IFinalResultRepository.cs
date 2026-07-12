using SchoolManagementSystem.Models.Entities.Result;

namespace SchoolManagementSystem.Repositories.Interfaces.Result;

public interface IFinalResultRepository : IBaseRepository<FinalResult>
{
    Task CalculateFinalPositionsBySpAsync(int academicYearId, CancellationToken ct = default);
}
