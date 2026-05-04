namespace SchoolManagementSystem.Helpers.Files;

public interface IFileStorageService
{
    Task<string> SaveAsync(IFormFile file, string moduleFolder, CancellationToken cancellationToken = default);
}
