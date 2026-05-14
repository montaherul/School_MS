using SchoolManagementSystem.Constants;
using SchoolManagementSystem.Services.Interfaces.Infrastructure;

namespace SchoolManagementSystem.Services.Implementations.Infrastructure;

/// <summary>
/// Concrete implementation of IFileStorageService.
/// Stores all files under wwwroot/uploads/{subfolder}/ with GUID-based names to prevent collisions.
/// </summary>
public class FileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<FileStorageService> _logger;

    public FileStorageService(IWebHostEnvironment env, ILogger<FileStorageService> logger)
    {
        _env    = env;
        _logger = logger;
    }

    public async Task<string> SaveAsync(IFormFile file, string subfolder, CancellationToken ct = default)
        => await SaveAsync(file, subfolder, string.Empty, ct);

    public async Task<string> SaveAsync(IFormFile file, string subfolder, string prefix, CancellationToken ct = default)
    {
        var ext      = Path.GetExtension(file.FileName).ToLowerInvariant();
        var fileName = string.IsNullOrWhiteSpace(prefix)
            ? $"{Guid.NewGuid()}{ext}"
            : $"{prefix}_{Guid.NewGuid()}{ext}";

        var folder = Path.Combine(_env.WebRootPath, "uploads", subfolder);
        Directory.CreateDirectory(folder);

        var filePath = Path.Combine(folder, fileName);
        await using var stream = File.Create(filePath);
        await file.CopyToAsync(stream, ct);

        _logger.LogInformation("File saved: /uploads/{Subfolder}/{FileName}", subfolder, fileName);
        return $"/uploads/{subfolder}/{fileName}".Replace("\\", "/");
    }

    public Task<bool> DeleteAsync(string relativePath, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(relativePath)) return Task.FromResult(false);

        var absolute = GetAbsolutePath(relativePath);
        if (!File.Exists(absolute)) return Task.FromResult(false);

        File.Delete(absolute);
        _logger.LogInformation("File deleted: {RelativePath}", relativePath);
        return Task.FromResult(true);
    }

    public ValidationResult Validate(IFormFile file, string[] allowedExtensions, long maxSizeBytes)
    {
        if (file == null || file.Length == 0)
            return ValidationResult.Fail("No file was provided.");

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(ext))
            return ValidationResult.Fail($"File type '{ext}' is not allowed. Allowed: {string.Join(", ", allowedExtensions)}");

        if (file.Length > maxSizeBytes)
            return ValidationResult.Fail($"File size exceeds the limit of {maxSizeBytes / 1024 / 1024} MB.");

        return ValidationResult.Ok();
    }

    public string GetAbsolutePath(string relativePath)
    {
        var normalized = relativePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString());
        return Path.Combine(_env.WebRootPath, normalized);
    }
}
