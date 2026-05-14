using SchoolManagementSystem.Constants;

namespace SchoolManagementSystem.Services.Interfaces.Infrastructure;

/// <summary>
/// Centralized file storage service. Replaces scattered upload logic across modules.
/// All uploads go through wwwroot/uploads/{subfolder}/.
/// </summary>
public interface IFileStorageService
{
    /// <summary>Saves an uploaded file; returns the relative URL path (e.g. /uploads/employees/abc.jpg).</summary>
    Task<string> SaveAsync(IFormFile file, string subfolder, CancellationToken ct = default);

    /// <summary>Saves an uploaded file with a specific filename prefix.</summary>
    Task<string> SaveAsync(IFormFile file, string subfolder, string prefix, CancellationToken ct = default);

    /// <summary>Deletes a file by its relative URL path. Returns false if file did not exist.</summary>
    Task<bool> DeleteAsync(string relativePath, CancellationToken ct = default);

    /// <summary>Validates an uploaded file against extension and size constraints.</summary>
    ValidationResult Validate(IFormFile file, string[] allowedExtensions, long maxSizeBytes);

    /// <summary>Returns the absolute disk path for a given relative URL path.</summary>
    string GetAbsolutePath(string relativePath);
}

public record ValidationResult(bool IsValid, string? Error = null)
{
    public static ValidationResult Ok() => new(true);
    public static ValidationResult Fail(string error) => new(false, error);
}
