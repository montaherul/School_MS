namespace SchoolManagementSystem.Helpers;

public static class IdCardImageHelper
{
    public static string FileToDataUri(string? relativePath, string baseFileUrl)
    {
        if (string.IsNullOrEmpty(relativePath))
            return string.Empty;

        var wwwRoot = baseFileUrl;
        if (wwwRoot.StartsWith("file:///"))
            wwwRoot = wwwRoot[8..];
        if (wwwRoot.EndsWith("/"))
            wwwRoot = wwwRoot[..^1];

        var fullPath = Path.Combine(wwwRoot, relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        if (!File.Exists(fullPath))
            return string.Empty;

        var bytes = File.ReadAllBytes(fullPath);
        var ext = Path.GetExtension(fullPath).ToLowerInvariant();
        var mime = ext switch
        {
            ".png" => "image/png",
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            ".svg" => "image/svg+xml",
            _ => "image/png"
        };

        return $"data:{mime};base64,{Convert.ToBase64String(bytes)}";
    }

    public static string GetBloodWithColor(string? bloodGroup)
    {
        if (string.IsNullOrEmpty(bloodGroup))
            return "N/A";
        return bloodGroup.ToUpperInvariant();
    }
}
