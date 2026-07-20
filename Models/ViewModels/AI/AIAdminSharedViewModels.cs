using SchoolManagementSystem.Models.DTOs.AI;

namespace SchoolManagementSystem.Models.ViewModels.AI;

public class AIAdminListViewModel<T>
{
    public List<T> Items { get; set; } = [];
    public int Page { get; set; }
    public int TotalPages { get; set; }
    public int TotalRecords { get; set; }
    public string Search { get; set; } = string.Empty;
    public string SortBy { get; set; } = string.Empty;
    public string SortDir { get; set; } = "asc";
}

public class AIKnowledgeIndexViewModel
{
    public List<AIKnowledgeBaseDto> Items { get; set; } = [];
}

public class AIAuditLogIndexViewModel
{
    public List<AIAuditLogDto> Items { get; set; } = [];
    public int Page { get; set; }
    public int TotalPages { get; set; }
    public int TotalRecords { get; set; }
}

public class AIHealthIndexViewModel
{
    public List<AIHealthCheckDto> Checks { get; set; } = [];
}

public class AIConversationAdminIndexViewModel
{
    public List<AIConversationAdminDto> Items { get; set; } = [];
    public int Page { get; set; }
    public int TotalPages { get; set; }
    public int TotalRecords { get; set; }
    public string Search { get; set; } = string.Empty;
    public string StatusFilter { get; set; } = string.Empty;
}
