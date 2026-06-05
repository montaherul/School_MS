using SchoolManagementSystem.Models.DTOs.Guardian;

namespace SchoolManagementSystem.Services.Guardian;

public interface IGuardianService
{
    Task<(IEnumerable<GuardianListItemDto> Items, int TotalCount)> GetGuardianListAsync(string? searchTerm, string? status, int pageNumber, int pageSize);
    Task<GuardianDetailsDto?> GetGuardianByIdAsync(int id);
    Task<int> CreateGuardianAsync(GuardianUpsertDto dto);
    Task UpdateGuardianAsync(GuardianUpsertDto dto);
    Task DeleteGuardianAsync(int id);
    Task SetGuardianStatusAsync(int id, bool active);
    Task LinkStudentAsync(int guardianId, int studentId, string relation);
    Task<GuardianDashboardDataDto> GetDashboardAsync(int guardianId);
    Task<GuardianDashboardDataDto> GetDashboardByUserIdAsync(int userId);
}
