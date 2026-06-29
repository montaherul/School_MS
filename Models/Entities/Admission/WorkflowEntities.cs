using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Entities.Base;
using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Models.Entities.Admission;

public class WorkflowDefinition : BaseEntity
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public int SortOrder { get; set; }

    public ICollection<WorkflowTransition> Transitions { get; set; } = new List<WorkflowTransition>();
}

public class WorkflowTransition : BaseEntity
{
    public int WorkflowDefinitionId { get; set; }
    public WorkflowDefinition WorkflowDefinition { get; set; } = null!;

    public WorkflowState FromState { get; set; }
    public WorkflowState ToState { get; set; }

    public WorkflowTransitionType TransitionType { get; set; }

    [MaxLength(200)]
    public string? RequiredPermission { get; set; }

    [MaxLength(500)]
    public string? ConditionExpression { get; set; }

    public bool RequiresApproval { get; set; }

    public int? RequiredApprovalCount { get; set; }

    [MaxLength(200)]
    public string? RequiredRole { get; set; }

    public int SortOrder { get; set; }

    public bool IsActive { get; set; } = true;
}

public class WorkflowInstance : BaseEntity
{
    public int AdmissionApplicationId { get; set; }
    public AdmissionApplication AdmissionApplication { get; set; } = null!;

    public int WorkflowDefinitionId { get; set; }
    public WorkflowDefinition WorkflowDefinition { get; set; } = null!;

    public WorkflowState CurrentState { get; set; }

    public bool IsCompleted { get; set; }

    public DateTime? CompletedAt { get; set; }

    public ICollection<WorkflowHistoryEntry> History { get; set; } = new List<WorkflowHistoryEntry>();
}

public class WorkflowHistoryEntry : BaseEntity
{
    public int WorkflowInstanceId { get; set; }
    public WorkflowInstance WorkflowInstance { get; set; } = null!;

    public WorkflowState FromState { get; set; }
    public WorkflowState ToState { get; set; }

    [MaxLength(500)]
    public string? Remarks { get; set; }

    public bool IsApproval { get; set; }

    [MaxLength(64)]
    public string? ActionedBy { get; set; }

    [MaxLength(200)]
    public string? ActionedByRole { get; set; }

    public DateTime ActionedAt { get; set; } = DateTime.UtcNow;

    // For rollback support
    public bool IsRolledBack { get; set; }
    public DateTime? RolledBackAt { get; set; }
    [MaxLength(64)]
    public string? RolledBackBy { get; set; }
}
