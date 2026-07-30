using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Repositories.Interfaces.Fees;
using SchoolManagementSystem.Services.Interfaces.Admin;
using SchoolManagementSystem.Services.Interfaces.Fees;

namespace SchoolManagementSystem.Services.Implementations.Fees;

public class ScholarshipEngineService : IScholarshipEngineService
{
    private readonly IScholarshipBatchRepository _repository;
    private readonly IAuditLogService _audit;

    public ScholarshipEngineService(IScholarshipBatchRepository repository, IAuditLogService audit)
    {
        _repository = repository;
        _audit = audit;
    }

    public async Task<ScholarshipEngineResultDto> RunAsync(CancellationToken ct = default)
    {
        var result = await _repository.ApplyScholarshipsAsync(ct);

        await _audit.LogAsync("ScholarshipEngine", "Run",
            $"Scholarship engine run: {result.ScholarshipsApplied} scholarship(s) applied to {result.StudentsProcessed} student(s), total {result.TotalDiscountAmount}", "system", cancellationToken: ct);

        return result;
    }
}
