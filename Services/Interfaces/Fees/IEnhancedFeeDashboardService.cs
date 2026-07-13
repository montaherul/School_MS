using SchoolManagementSystem.Models.DTOs.Fees;

namespace SchoolManagementSystem.Services.Interfaces.Fees;

public interface IEnhancedFeeDashboardService
{
    Task<EnhancedFeeDashboardDto> GetDashboardAsync(int? academicYearId = null);
}
