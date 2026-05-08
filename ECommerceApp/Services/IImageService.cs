namespace ECommerceApp.Services;

public interface IImageService
{
    Task<string> SaveProductImageAsync(IFormFile file, int productId);
    Task<string> SaveProductThumbnailAsync(IFormFile file, int productId);
    void DeleteImage(string relativePath);
}
