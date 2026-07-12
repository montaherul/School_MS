using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.Entities.Academic;

namespace SchoolManagementSystem.Services.Interfaces.Academic;

public interface IAcademicYearService
{
    Task<PagedResult<AcademicYearListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default);
    Task<AcademicYearUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default);
    Task<AcademicYear?> GetActiveYearAsync(CancellationToken ct = default);
    Task<IReadOnlyList<AcademicYear>> GetAllYearsAsync(CancellationToken ct = default);
    Task<AcademicYear?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<int> CreateAsync(AcademicYearUpsertDto dto, string createdBy, CancellationToken cancellationToken = default);
    Task UpdateAsync(AcademicYearUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default);
}

