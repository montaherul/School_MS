using SchoolManagementSystem.Models.DTOs.Result;

namespace SchoolManagementSystem.Services.Interfaces.Result;

public interface IPromotionWizardService
{
    Task<PromotionWizardPreviewDto> GetPreviewAsync(int fromAcademicYearId, int fromClassId, int toClassId, int? examId, CancellationToken ct = default);
    Task<PromotionWizardExecuteResult> ExecuteAsync(PromotionWizardExecuteRequest request, int userId, CancellationToken ct = default);
}
