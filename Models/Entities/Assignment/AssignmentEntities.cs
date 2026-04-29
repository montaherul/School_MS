using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Entities.Base;
using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Models.Entities.Assignment;

public class AssignmentTask : BaseEntity
{
    public int SchoolClassId { get; set; }
    public int SectionId { get; set; }
    public int SubjectId { get; set; }
    public int TeacherProfileId { get; set; }

    [MaxLength(160)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string Instructions { get; set; } = string.Empty;

    public DateTime Deadline { get; set; }
    public AssignmentStatus Status { get; set; } = AssignmentStatus.Open;

    [MaxLength(260)]
    public string? AttachmentPath { get; set; }
}

public class AssignmentSubmission : BaseEntity
{
    public int AssignmentTaskId { get; set; }
    public AssignmentTask? AssignmentTask { get; set; }
    public int StudentId { get; set; }

    [MaxLength(260)]
    public string FilePath { get; set; } = string.Empty;

    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public decimal? Marks { get; set; }

    [MaxLength(1000)]
    public string? Feedback { get; set; }
}
