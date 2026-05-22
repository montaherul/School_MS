using System.Collections.Generic;
using SchoolManagementSystem.Models.Entities.Communication;

namespace SchoolManagementSystem.Models.ViewModels.Communication;

public class NoticeListViewModel
{
    public IReadOnlyList<Notice> Notices { get; set; } = new List<Notice>();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public string? Search { get; set; }
}
