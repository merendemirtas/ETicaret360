namespace ECommerceApp.Helpers;

// Yüklenen dosya / görsel işlemleri için yardımcı metotlar
public static class FileHelper
{
    // İzin verilen görsel uzantıları (büyük/küçük harf duyarsız karşılaştırılır)
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];

    // Maksimum dosya boyutu: 5 MB
    private const long MaxFileSizeBytes = 5 * 1024 * 1024;

    // Yüklenen dosyanın boyut ve uzantı bakımından geçerli bir görsel olup olmadığını döner
    public static bool IsValidImage(IFormFile file)
    {
        if (file.Length > MaxFileSizeBytes) return false;
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        return AllowedExtensions.Contains(ext);
    }

    // wwwroot içindeki bir dosyayı göreli yola göre fiziksel olarak siler
    public static void DeleteFile(string relativePath, IWebHostEnvironment env)
    {
        if (string.IsNullOrEmpty(relativePath)) return;
        var fullPath = Path.Combine(env.WebRootPath, relativePath.TrimStart('/'));
        if (File.Exists(fullPath))
            File.Delete(fullPath);
    }
}
