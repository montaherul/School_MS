using SchoolManagementSystem.Models.Entities.Guardian;
using SchoolManagementSystem.Models.DTOs.Guardian;
using SchoolManagementSystem.Repositories.Interfaces;

namespace SchoolManagementSystem.Repositories.Guardian;

public interface IGuardianRepository : IBaseRepository<SchoolManagementSystem.Models.Entities.Guardian.Guardian>
{
    Task<(IEnumerable<GuardianListItemDto> Items, int TotalCount)> GetListAsync(string? searchTerm, string? status, int pageNumber, int pageSize);
    Task<GuardianDetailsDto?> GetDetailsAsync(int guardianId);
    Task<GuardianDashboardDataDto> GetDashboardDataAsync(int guardianId);
    Task<bool> IsCodeUniqueAsync(string code);
    Task<bool> IsMobileUniqueAsync(string mobile);
    Task<int> GetStudentCountAsync(int guardianId);
    Task LinkStudentAsync(int guardianId, int studentId, string relation);
}
