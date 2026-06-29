using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Entities.Base;

namespace SchoolManagementSystem.Models.Entities.Website;

public enum EventApprovalStatus
{
    Draft = 0,
    PendingApproval = 1,
    Approved = 2,
    Rejected = 3
}

public class Event : BaseEntity
{
    [MaxLength(200)]
    public string? Title { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public DateTime EventDate { get; set; }

    [MaxLength(500)]
    public string? EventLocation { get; set; }

    public bool IsUpcoming { get; set; } = false;

    public bool IsPublished { get; set; } = false;

    public EventApprovalStatus ApprovalStatus { get; set; } = EventApprovalStatus.Draft;

    public int? ApprovedBy { get; set; }

    public DateTime? ApprovedAt { get; set; }

    [MaxLength(500)]
    public string? RejectionReason { get; set; }

    [MaxLength(200)]
    public string? CoverImagePath { get; set; }

    public int? EventCategoryId { get; set; }
    public EventCategory? EventCategory { get; set; }
}