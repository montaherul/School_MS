using SchoolManagementSystem.Models.Entities.Admission;
using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Repositories.Interfaces.Admission;

public interface IWorkflowRepository : IBaseRepository<WorkflowDefinition>
{
    Task<List<WorkflowDefinition>> GetActiveWorkflowsAsync(CancellationToken ct = default);
    Task<List<WorkflowTransition>> GetTransitionsForStateAsync(int workflowDefinitionId, WorkflowState fromState, CancellationToken ct = default);
    Task<WorkflowInstance?> GetInstanceByApplicationIdAsync(int applicationId, CancellationToken ct = default);
    Task<List<WorkflowHistoryEntry>> GetHistoryByApplicationIdAsync(int applicationId, CancellationToken ct = default);
}
