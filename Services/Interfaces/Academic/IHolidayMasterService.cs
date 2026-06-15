using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.DTOs.Common;

namespace SchoolManagementSystem.Services.Interfaces.Academic;

public interface IHolidayMasterService
{
    Task<PagedResult<HolidayMasterDto>> GetPagedAsync(int page, int pageSize, string? search, string? type, string? religion, CancellationToken ct = default);
    Task<HolidayMasterDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<int> CreateAsync(HolidayMasterUpsertDto dto, string createdBy, CancellationToken ct = default);
    Task UpdateAsync(HolidayMasterUpsertDto dto, string updatedBy, CancellationToken ct = default);
    Task DeleteAsync(int id, string updatedBy, CancellationToken ct = default);
    Task ActivateAsync(int id, string updatedBy, CancellationToken ct = default);
    Task DeactivateAsync(int id, string updatedBy, CancellationToken ct = default);
    Task<int> ImportAsync(List<HolidayMasterUpsertDto> holidays, string createdBy, CancellationToken ct = default);
    Task<byte[]> ExportAsync(CancellationToken ct = default);
    Task<List<HolidayMasterDto>> GetAllAsync(CancellationToken ct = default);
}
