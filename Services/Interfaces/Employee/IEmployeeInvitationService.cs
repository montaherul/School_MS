using SchoolManagementSystem.Models.DTOs.Employee;

namespace SchoolManagementSystem.Services.Interfaces.Employee;

public interface IEmployeeInvitationService
{
    Task<(List<EmployeeInvitationDto> items, int totalRecords)> GetPagedInvitationsAsync(int page, int pageSize, string? search, CancellationToken ct);
    Task<EmployeeInvitationDto?> GetInvitationByIdAsync(int id, CancellationToken ct);
    Task<EmployeeInvitationDto?> GetInvitationByTokenAsync(string token, CancellationToken ct);
    Task<int> CreateInvitationAsync(EmployeeInvitationUpsertDto dto, int createdByUserId, CancellationToken ct);
    Task<bool> ResendInvitationAsync(int id, CancellationToken ct);
    Task<bool> CancelInvitationAsync(int id, CancellationToken ct);
    Task<bool> ValidateTokenAsync(string token, CancellationToken ct);
    Task<bool> MarkInvitationOpenedAsync(string token, CancellationToken ct);
    Task<(bool success, string message)> CompleteOnboardingAsync(EmployeeUpsertDto model, string token, string password, CancellationToken ct);
    Task<bool> ApproveOnboardingAsync(int id, int approvedByUserId, CancellationToken ct);
}
