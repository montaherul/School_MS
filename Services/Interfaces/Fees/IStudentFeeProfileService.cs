using SchoolManagementSystem.Models.DTOs.Fees;

namespace SchoolManagementSystem.Services.Interfaces.Fees;

public interface IStudentFeeProfileService
{
    Task<StudentFeeProfileDto> GetProfileAsync(int studentId, int? academicYearId = null);
    Task<List<StudentSearchResultDto>> SearchStudentsAsync(string? term);
    Task<byte[]> GenerateProfilePdfAsync(int studentId, int? academicYearId = null);
}
