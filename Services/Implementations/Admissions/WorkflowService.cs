using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Models.DTOs.Admission;
using SchoolManagementSystem.Models.Entities.Admission;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.Admission;
using SchoolManagementSystem.Services.Interfaces.Admissions;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Admissions;

public class WorkflowService : IWorkflowService
{
    private readonly IWorkflowRepository _workflowRepository;
    private readonly IAdmissionRepository _admissionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<WorkflowService> _logger;

    public WorkflowService(
        IWorkflowRepository workflowRepository,
        IAdmissionRepository admissionRepository,
        IUnitOfWork unitOfWork,
        ILogger<WorkflowService> logger)
    {
        _workflowRepository = workflowRepository;
        _admissionRepository = admissionRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<WorkflowInstance> InitializeWorkflowAsync(int applicationId, CancellationToken ct = default)
    {
        var existing = await _workflowRepository.GetInstanceByApplicationIdAsync(applicationId, ct);
        if (existing != null && !existing.IsCompleted)
            return existing;

        var definitions = await _workflowRepository.GetActiveWorkflowsAsync(ct);
        var definition = definitions.FirstOrDefault() ?? throw new InvalidOperationException("No active workflow definition found.");

        var instance = new WorkflowInstance
        {
            AdmissionApplicationId = applicationId,
            WorkflowDefinitionId = definition.Id,
            CurrentState = WorkflowState.ApplicationSubmitted,
            IsCompleted = false,
            CreatedBy = "system",
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Repository<WorkflowInstance>().AddAsync(instance, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        await AddHistoryEntryAsync(instance.Id, WorkflowState.ApplicationSubmitted, WorkflowState.ApplicationSubmitted, "Workflow initialized", "system", null, false, ct);

        return instance;
    }

    public async Task<WorkflowInstance> TransitionAsync(int applicationId, WorkflowState targetState, string actionedBy, string? remarks = null, CancellationToken ct = default)
    {
        var instance = await _workflowRepository.GetInstanceByApplicationIdAsync(applicationId, ct)
            ?? throw new InvalidOperationException("No workflow instance found for this application.");

        if (instance.IsCompleted)
            throw new InvalidOperationException("Workflow is already completed.");

        var allowed = await GetAllowedTransitionsAsync(applicationId, ct);
        if (!allowed.Any(t => t.ToState == targetState))
            throw new InvalidOperationException($"Transition to {targetState} is not allowed from current state {instance.CurrentState}.");

        var fromState = instance.CurrentState;
        instance.CurrentState = targetState;

        if (targetState == WorkflowState.AdmissionCompleted || targetState == WorkflowState.Rejected || targetState == WorkflowState.Cancelled)
        {
            instance.IsCompleted = true;
            instance.CompletedAt = DateTime.UtcNow;
        }

        _unitOfWork.Repository<WorkflowInstance>().Update(instance);

        await AddHistoryEntryAsync(instance.Id, fromState, targetState, remarks, actionedBy, null, false, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        // Update admission application status
        var application = await _admissionRepository.FirstOrDefaultAsync(x => x.Id == applicationId && !x.IsDeleted, ct);
        if (application != null)
        {
            application.Status = MapWorkflowStateToAdmissionStatus(targetState);
            application.UpdatedBy = actionedBy;
            application.UpdatedAt = DateTime.UtcNow;
            _admissionRepository.Update(application);
            await _unitOfWork.SaveChangesAsync(ct);
        }

        return instance;
    }

    public async Task<AdmissionTimelineDto> GetTimelineAsync(int applicationId, CancellationToken ct = default)
    {
        var instance = await _workflowRepository.GetInstanceByApplicationIdAsync(applicationId, ct);
        var application = await _admissionRepository.FirstOrDefaultAsync(x => x.Id == applicationId && !x.IsDeleted, ct);

        if (application == null)
            throw new InvalidOperationException("Application not found.");

        var history = await _workflowRepository.GetHistoryByApplicationIdAsync(applicationId, ct);

        var timeline = new AdmissionTimelineDto
        {
            ApplicationId = applicationId,
            ApplicationNo = application.ApplicationNo,
            CurrentState = instance?.CurrentState.ToString() ?? "Unknown",
            Timeline = history.Select(h => new WorkflowTimelineEntry
            {
                Id = h.Id,
                FromState = h.FromState.ToString(),
                ToState = h.ToState.ToString(),
                Remarks = h.Remarks,
                ActionedBy = h.ActionedBy,
                ActionedByRole = h.ActionedByRole,
                ActionedAt = h.ActionedAt,
                IsRolledBack = h.IsRolledBack
            }).ToList(),
            AvailableTransitions = (await GetAllowedTransitionsAsync(applicationId, ct))
                .Select(t => new WorkflowStateDto
                {
                    State = t.ToState,
                    StateName = t.ToState.ToString(),
                    IsActive = true,
                    IsCurrent = false
                }).ToList()
        };

        return timeline;
    }

    public async Task<List<WorkflowTransition>> GetAllowedTransitionsAsync(int applicationId, CancellationToken ct = default)
    {
        var instance = await _workflowRepository.GetInstanceByApplicationIdAsync(applicationId, ct);
        if (instance == null || instance.IsCompleted)
            return new List<WorkflowTransition>();

        return await _workflowRepository.GetTransitionsForStateAsync(instance.WorkflowDefinitionId, instance.CurrentState, ct);
    }

    public async Task<bool> CanTransitionAsync(int applicationId, WorkflowState targetState, string userId, CancellationToken ct = default)
    {
        var transitions = await GetAllowedTransitionsAsync(applicationId, ct);
        var transition = transitions.FirstOrDefault(t => t.ToState == targetState);
        if (transition == null) return false;

        if (transition.RequiresApproval && !string.IsNullOrEmpty(transition.RequiredPermission))
        {
            // Permission check would go here - simplified for now
            return true;
        }

        return true;
    }

    public async Task<List<WorkflowStateDto>> GetAvailableStatesAsync(int applicationId, CancellationToken ct = default)
    {
        var timeline = await GetTimelineAsync(applicationId, ct);
        return timeline.AvailableTransitions;
    }

    public async Task RollbackAsync(int historyEntryId, string rolledBackBy, CancellationToken ct = default)
    {
        var entry = await _unitOfWork.Repository<WorkflowHistoryEntry>().FirstOrDefaultAsync(h => h.Id == historyEntryId && !h.IsDeleted, ct)
            ?? throw new InvalidOperationException("History entry not found.");

        if (entry.IsRolledBack)
            throw new InvalidOperationException("This action has already been rolled back.");

        entry.IsRolledBack = true;
        entry.RolledBackAt = DateTime.UtcNow;
        entry.RolledBackBy = rolledBackBy;

        var instance = await _unitOfWork.Repository<WorkflowInstance>().FirstOrDefaultAsync(i => i.Id == entry.WorkflowInstanceId && !i.IsDeleted, ct);
        if (instance != null)
        {
            instance.CurrentState = entry.FromState;
            instance.IsCompleted = false;
            instance.CompletedAt = null;
            _unitOfWork.Repository<WorkflowInstance>().Update(instance);
        }

        _unitOfWork.Repository<WorkflowHistoryEntry>().Update(entry);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    private async Task AddHistoryEntryAsync(int workflowInstanceId, WorkflowState fromState, WorkflowState toState, string? remarks, string actionedBy, string? actionedByRole, bool isApproval, CancellationToken ct)
    {
        var entry = new WorkflowHistoryEntry
        {
            WorkflowInstanceId = workflowInstanceId,
            FromState = fromState,
            ToState = toState,
            Remarks = remarks,
            ActionedBy = actionedBy,
            ActionedByRole = actionedByRole,
            ActionedAt = DateTime.UtcNow,
            IsApproval = isApproval,
            CreatedBy = actionedBy,
            CreatedAt = DateTime.UtcNow
        };
        await _unitOfWork.Repository<WorkflowHistoryEntry>().AddAsync(entry, ct);
    }

    public async Task LogPipelineStepAsync(int applicationId, string stepName, string actionedBy, string? remarks = null, CancellationToken ct = default)
    {
        try
        {
            var instance = await _workflowRepository.GetInstanceByApplicationIdAsync(applicationId, ct);
            if (instance == null) return;

            await AddHistoryEntryAsync(instance.Id, instance.CurrentState, instance.CurrentState,
                $"[Pipeline:{stepName}] {remarks ?? stepName}", actionedBy, null, false, ct);

            if (stepName == "AdmissionCompleted")
            {
                instance.CurrentState = WorkflowState.AdmissionCompleted;
                instance.IsCompleted = true;
                instance.CompletedAt = DateTime.UtcNow;
                _unitOfWork.Repository<WorkflowInstance>().Update(instance);
            }

            await _unitOfWork.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to log pipeline step {Step} for application {AppId}", stepName, applicationId);
        }
    }

    private static AdmissionStatus MapWorkflowStateToAdmissionStatus(WorkflowState state)
    {
        return state switch
        {
            WorkflowState.Rejected => AdmissionStatus.Rejected,
            WorkflowState.AdmissionCompleted => AdmissionStatus.Converted,
            WorkflowState.PrincipalApproval => AdmissionStatus.Approved,
            _ => AdmissionStatus.Pending
        };
    }
}
