namespace SchoolManagementSystem.Models.DTOs.Common;

/// <summary>
/// Standardized request for Tabulator remote pagination and filtering.
/// </summary>
public class GridRequestDto
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;
    public string? Search { get; set; }
    public string? SortField { get; set; }
    public string? SortDir { get; set; }
    
    // JSON Filter strings (optional for future)
    public string? Filters { get; set; }
}

/// <summary>
/// Standardized response for Tabulator.js.
/// </summary>
/// <typeparam name="T"></typeparam>
public class PagedGridResultDto<T>
{
    public List<T> Data { get; set; } = new();
    public int LastPage { get; set; }
    public int Total { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }

    public static PagedGridResultDto<T> Create(List<T> data, int total, int page, int size)
    {
        return new PagedGridResultDto<T>
        {
            Data = data,
            Total = total,
            CurrentPage = page,
            PageSize = size,
            LastPage = (int)Math.Ceiling(total / (double)size)
        };
    }
}
