using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Models.DTOs.Admission;

public class WorkflowTransitionRequest
{
    public int ApplicationId { get; set; }
    public WorkflowState TargetState { get; set; }
    public string? Remarks { get; set; }
}

public class WorkflowStateDto
{
    public WorkflowState State { get; set; }
    public string StateName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool IsCurrent { get; set; }
    public DateTime? EnteredAt { get; set; }
}

public class WorkflowTimelineEntry
{
    public int Id { get; set; }
    public string FromState { get; set; } = string.Empty;
    public string ToState { get; set; } = string.Empty;
    public string? Remarks { get; set; }
    public string? ActionedBy { get; set; }
    public string? ActionedByRole { get; set; }
    public DateTime ActionedAt { get; set; }
    public bool IsRolledBack { get; set; }
}

public class AdmissionTimelineDto
{
    public int ApplicationId { get; set; }
    public string ApplicationNo { get; set; } = string.Empty;
    public string CurrentState { get; set; } = string.Empty;
    public List<WorkflowTimelineEntry> Timeline { get; set; } = new();
    public List<WorkflowStateDto> AvailableTransitions { get; set; } = new();
}
