namespace ECommerceApp.Helpers;

public static class FileHelper
{
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];
    private const long MaxFileSizeBytes = 5 * 1024 * 1024;

    public static bool IsValidImage(IFormFile file)
    {
        if (file.Length > MaxFileSizeBytes) return false;
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        return AllowedExtensions.Contains(ext);
    }

    public static void DeleteFile(string relativePath, IWebHostEnvironment env)
    {
        if (string.IsNullOrEmpty(relativePath)) return;
        var fullPath = Path.Combine(env.WebRootPath, relativePath.TrimStart('/'));
        if (File.Exists(fullPath))
            File.Delete(fullPath);
    }
}
