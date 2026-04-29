namespace SchoolManagementSystem.Helpers.Files;

public class LocalFileStorageService : IFileStorageService
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".pdf", ".jpg", ".jpeg", ".png", ".doc", ".docx"
    };

    private readonly IWebHostEnvironment _environment;

    public LocalFileStorageService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string> SaveAsync(IFormFile file, string moduleFolder, CancellationToken cancellationToken = default)
    {
        if (file.Length <= 0 || file.Length > 5 * 1024 * 1024)
        {
            throw new InvalidOperationException("File size must be between 1 byte and 5 MB.");
        }

        var extension = Path.GetExtension(file.FileName);
        if (!AllowedExtensions.Contains(extension))
        {
            throw new InvalidOperationException("File type is not allowed.");
        }

        var safeFolder = string.Join("", moduleFolder.Where(char.IsLetterOrDigit));
        var relativeFolder = Path.Combine("uploads", safeFolder);
        var absoluteFolder = Path.Combine(_environment.WebRootPath, relativeFolder);
        Directory.CreateDirectory(absoluteFolder);

        var fileName = $"{Guid.NewGuid():N}{extension}";
        var absolutePath = Path.Combine(absoluteFolder, fileName);
        await using var stream = File.Create(absolutePath);
        await file.CopyToAsync(stream, cancellationToken);
        return "/" + Path.Combine(relativeFolder, fileName).Replace("\\", "/");
    }
}
