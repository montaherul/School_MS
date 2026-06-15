using SchoolManagementSystem.Models.DTOs.Identity;

namespace SchoolManagementSystem.Services.Interfaces.Identity;

public interface IIdCardService
{
    Task<(List<StudentIdCardListDto> Items, int TotalRecords)> GetStudentIdCardListAsync(
        int page, int pageSize, string? search,
        int? classId, int? sectionId, int? groupId,
        string? status, string? gender,
        DateTime? admissionFrom, DateTime? admissionTo,
        CancellationToken ct);

    Task<(List<EmployeeIdCardListDto> Items, int TotalRecords)> GetEmployeeIdCardListAsync(
        int page, int pageSize, string? search,
        int? departmentId, int? designationId,
        string? status, string? employmentType,
        DateTime? joiningFrom, DateTime? joiningTo,
        CancellationToken ct);

    Task<List<StudentIdCardBulkDto>> GetStudentIdCardBulkDataAsync(string ids, CancellationToken ct);

    Task<List<EmployeeIdCardBulkDto>> GetEmployeeIdCardBulkDataAsync(string ids, CancellationToken ct);
}
