using SchoolManagementSystem.Models.DTOs.Admission;
using SchoolManagementSystem.Models.Entities.Admission;
using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Services.Interfaces.Admissions;

public interface IWorkflowService
{
    Task<List<WorkflowStateDto>> GetAvailableStatesAsync(int applicationId, CancellationToken ct = default);
    Task<WorkflowInstance> TransitionAsync(int applicationId, WorkflowState targetState, string actionedBy, string? remarks = null, CancellationToken ct = default);
    Task<AdmissionTimelineDto> GetTimelineAsync(int applicationId, CancellationToken ct = default);
    Task<bool> CanTransitionAsync(int applicationId, WorkflowState targetState, string userId, CancellationToken ct = default);
    Task<List<WorkflowTransition>> GetAllowedTransitionsAsync(int applicationId, CancellationToken ct = default);
    Task RollbackAsync(int historyEntryId, string rolledBackBy, CancellationToken ct = default);
    Task<WorkflowInstance> InitializeWorkflowAsync(int applicationId, CancellationToken ct = default);
    Task LogPipelineStepAsync(int applicationId, string stepName, string actionedBy, string? remarks = null, CancellationToken ct = default);
}
