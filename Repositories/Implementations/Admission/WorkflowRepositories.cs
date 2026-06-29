using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Admission;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.Admission;

namespace SchoolManagementSystem.Repositories.Implementations.Admission;

public class WorkflowRepository : BaseRepository<WorkflowDefinition>, IWorkflowRepository
{
    public WorkflowRepository(SchoolDbContext db) : base(db) { }

    public async Task<List<WorkflowDefinition>> GetActiveWorkflowsAsync(CancellationToken ct = default)
    {
        return await Query().AsNoTracking()
            .Where(w => w.IsActive && !w.IsDeleted)
            .Include(w => w.Transitions.Where(t => t.IsActive && !t.IsDeleted))
            .OrderBy(w => w.SortOrder)
            .ToListAsync(ct);
    }

    public async Task<List<WorkflowTransition>> GetTransitionsForStateAsync(
        int workflowDefinitionId, WorkflowState fromState, CancellationToken ct = default)
    {
        return await _db.Set<WorkflowTransition>().AsNoTracking()
            .Where(t => t.WorkflowDefinitionId == workflowDefinitionId
                && t.FromState == fromState
                && t.IsActive && !t.IsDeleted)
            .OrderBy(t => t.SortOrder)
            .ToListAsync(ct);
    }

    public async Task<WorkflowInstance?> GetInstanceByApplicationIdAsync(int applicationId, CancellationToken ct = default)
    {
        return await _db.Set<WorkflowInstance>()
            .Include(i => i.History.OrderByDescending(h => h.ActionedAt))
            .FirstOrDefaultAsync(i => i.AdmissionApplicationId == applicationId && !i.IsDeleted, ct);
    }

    public async Task<List<WorkflowHistoryEntry>> GetHistoryByApplicationIdAsync(int applicationId, CancellationToken ct = default)
    {
        return await _db.Set<WorkflowHistoryEntry>().AsNoTracking()
            .Where(h => h.WorkflowInstance.AdmissionApplicationId == applicationId && !h.IsDeleted)
            .OrderByDescending(h => h.ActionedAt)
            .ToListAsync(ct);
    }
}
