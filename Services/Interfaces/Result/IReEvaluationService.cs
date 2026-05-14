using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.ViewModels.Result;

namespace SchoolManagementSystem.Services.Interfaces.Result;

public interface IReEvaluationService
{
    Task RequestReEvaluationAsync(ReEvaluationRequestDto dto, int requestedByUserId);
    Task ProcessReEvaluationAsync(ReEvaluationProcessDto dto, int adminId);
    Task<ReEvaluationDashboardViewModel> GetReEvaluationDashboardAsync();
}

