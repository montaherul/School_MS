using SchoolManagementSystem.Models.DTOs.Result;

namespace SchoolManagementSystem.Services.Interfaces.Result;

public interface IReEvaluationService
{
    Task RequestReEvaluationAsync(ReEvaluationRequestDto dto, int requestedByUserId);
    Task ProcessReEvaluationAsync(ReEvaluationProcessDto dto, int adminId);
    Task<ReEvaluationDashboardDto> GetReEvaluationDashboardAsync();
}

