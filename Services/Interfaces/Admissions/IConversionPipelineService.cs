using SchoolManagementSystem.Models.DTOs.Admission;
using SchoolManagementSystem.Models.Entities.Admission;
using SchoolManagementSystem.Models.Entities.Auth;

namespace SchoolManagementSystem.Services.Interfaces.Admissions;

public interface IConversionPipelineService
{
    Task<ConversionResult> ExecuteAsync(int applicationId, int sectionId, string approvedBy, CancellationToken ct = default);
    Task<AdmissionApplication?> ValidateAsync(int applicationId, CancellationToken ct = default);
    Task<(ApplicationUser user, string activationToken)> CreateUserAsync(int applicationId, string approvedBy, CancellationToken ct = default);
    Task<int> GenerateRollNumberAsync(int classId, int sectionId, CancellationToken ct = default);
    Task<int?> ResolveGroupAsync(int applicationId, int sectionId, int classSortOrder, int groupStartClass, CancellationToken ct = default);
    Task<(Models.Entities.Guardian.Guardian? guardian, string? activationToken)> CreateGuardianAsync(int applicationId, bool portalEnabled, bool activationEnabled, string approvedBy, CancellationToken ct = default);
    Task<int> CreateStudentAsync(int applicationId, int sectionId, int? groupId, int rollNumber, int userId, string approvedBy, CancellationToken ct = default);
    Task CreateFeeInvoiceAsync(int applicationId, int studentId, string approvedBy, CancellationToken ct = default);
    Task SendEmailsAsync(int applicationId, int userId, bool portalEnabled, bool activationEnabled, string? guardianActivationToken, string? guardianEmail, string? guardianFullName, string? guardianCode, CancellationToken ct = default);
}
