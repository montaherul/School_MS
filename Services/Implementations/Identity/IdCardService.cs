using SchoolManagementSystem.Models.DTOs.Identity;
using SchoolManagementSystem.Repositories.Interfaces.Identity;
using SchoolManagementSystem.Services.Interfaces.Identity;

namespace SchoolManagementSystem.Services.Implementations.Identity;

public class IdCardService : IIdCardService
{
    private readonly IIdCardRepository _idCardRepository;

    public IdCardService(IIdCardRepository idCardRepository)
    {
        _idCardRepository = idCardRepository;
    }

    public async Task<(List<StudentIdCardListDto> Items, int TotalRecords)> GetStudentIdCardListAsync(
        int page, int pageSize, string? search,
        int? classId, int? sectionId, int? groupId,
        string? status, string? gender,
        DateTime? admissionFrom, DateTime? admissionTo,
        CancellationToken ct)
    {
        return await _idCardRepository.GetStudentIdCardListAsync(
            page, pageSize, search, classId, sectionId, groupId,
            status, gender, admissionFrom, admissionTo, ct);
    }

    public async Task<(List<EmployeeIdCardListDto> Items, int TotalRecords)> GetEmployeeIdCardListAsync(
        int page, int pageSize, string? search,
        int? departmentId, int? designationId,
        string? status, string? employmentType,
        DateTime? joiningFrom, DateTime? joiningTo,
        CancellationToken ct)
    {
        return await _idCardRepository.GetEmployeeIdCardListAsync(
            page, pageSize, search, departmentId, designationId,
            status, employmentType, joiningFrom, joiningTo, ct);
    }
}
