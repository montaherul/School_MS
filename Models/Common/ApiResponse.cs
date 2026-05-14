namespace SchoolManagementSystem.Models.Common;

/// <summary>
/// Unified API response wrapper used by all AJAX endpoints.
/// Provides consistent JSON shape: { success, message, data, errors }
/// </summary>
public class ApiResponse<T>
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public T? Data { get; init; }
    public IReadOnlyList<string> Errors { get; init; } = [];

    public static ApiResponse<T> Ok(T data, string message = "Success")
        => new() { Success = true, Message = message, Data = data };

    public static ApiResponse<T> Fail(string message, IEnumerable<string>? errors = null)
        => new() { Success = false, Message = message, Errors = errors?.ToList() ?? [] };

    public static ApiResponse<T> ValidationFail(IEnumerable<string> errors)
        => new() { Success = false, Message = "Validation failed.", Errors = errors.ToList() };
}

/// <summary>Non-generic shorthand for operations that return no data payload.</summary>
public class ApiResponse : ApiResponse<object?>
{
    public static ApiResponse Ok(string message = "Success")
        => new() { Success = true, Message = message };

    public static new ApiResponse Fail(string message, IEnumerable<string>? errors = null)
        => new() { Success = false, Message = message, Errors = errors?.ToList() ?? [] };
}

/// <summary>
/// Standard paginated result. Already used across services — this adds helper factory methods.
/// </summary>
public class PagedApiResponse<T>
{
    public bool Success { get; init; } = true;
    public string Message { get; init; } = "Success";
    public IReadOnlyList<T> Items { get; init; } = [];
    public int TotalItems { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalItems / (double)PageSize) : 0;
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
}
