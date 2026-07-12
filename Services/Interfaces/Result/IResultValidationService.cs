using SchoolManagementSystem.Models.DTOs.Result;

namespace SchoolManagementSystem.Services.Interfaces.Result;

public interface IResultValidationService
{
    Task<ResultValidationResultDto> ValidateAsync(ResultValidationRequest request, CancellationToken ct = default);
    Task<ResultValidationResultDto> ValidatePrePublicationAsync(int examId, CancellationToken ct = default);
}
