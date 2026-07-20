namespace SchoolManagementSystem.Models.Common;

public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public T? Data { get; private set; }
    public string? ErrorMessage { get; private set; }
    public string? ErrorCode { get; private set; }

    public bool IsFailure => !IsSuccess;

    public static Result<T> Success(T data) => new() { IsSuccess = true, Data = data };
    public static Result<T> Fail(string message, string? code = null) => new() { IsSuccess = false, ErrorMessage = message, ErrorCode = code };

    public T ValueOrDefault(T defaultValue) => IsSuccess && Data is not null ? Data : defaultValue;
}

public static class Result
{
    public static Result<T> Success<T>(T data) => Result<T>.Success(data);
    public static Result<T> Fail<T>(string message, string? code = null) => Result<T>.Fail(message, code);
}
