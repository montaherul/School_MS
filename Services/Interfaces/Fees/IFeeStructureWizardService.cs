using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.ViewModels.Fees;

namespace SchoolManagementSystem.Services.Interfaces.Fees;

public interface IFeeStructureWizardService
{
    Task<FeeStructureWizardViewModel> GetWizardDataAsync(FeeStructureWizardDto? state = null);
    Task<AutoBillingResultDto> SaveWizardAsync(FeeStructureWizardDto wizard, string createdBy);
}
