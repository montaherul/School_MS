using SchoolManagementSystem.Models.Entities.Auth;

namespace SchoolManagementSystem.Models.ViewModels.Admin;

public class AuditLogListItemViewModel
{
    public int Id { get; set; }
    public string Module { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public string? Details { get; set; }
    public string? UserName { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AuditLogIndexViewModel
{
    public List<AuditLogListItemViewModel> Items { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public string? Search { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
}
