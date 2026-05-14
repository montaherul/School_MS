namespace SchoolManagementSystem.Models.DTOs.Common;

public class PagedApiResponse<T>
{
    public IEnumerable<T> data { get; set; } = new List<T>();
    public int last_page { get; set; }
    public int total { get; set; }
    public string? error { get; set; }
}
