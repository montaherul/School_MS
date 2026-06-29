using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models.DTOs.Admission;

public class BulkIdsRequest
{
    [Required, MinLength(1)]
    public List<int> Ids { get; set; } = new();
}

public class BulkOperationProgress
{
    public int Total { get; set; }
    public int Processed { get; set; }
    public int Succeeded { get; set; }
    public int Failed { get; set; }
    public string? CurrentItem { get; set; }
    public List<string> Errors { get; set; } = new();
    public bool IsCompleted { get; set; }
}

public class BulkAssignRequest : BulkIdsRequest
{
    public int? ClassId { get; set; }
    public int? SectionId { get; set; }
    public int? GroupId { get; set; }
}

public class BulkDocumentActionRequest : BulkIdsRequest
{
    public string DocumentType { get; set; } = string.Empty;
    public bool Verify { get; set; }
    public string? Remarks { get; set; }
}
