using ECommerceApp.Data;
using ECommerceApp.Helpers;
using ECommerceApp.Models.Entities;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace ECommerceApp.Services;

public class ImageService : IImageService
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;

    public ImageService(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    public async Task<string> SaveProductImageAsync(IFormFile file, int productId)
    {
        if (!FileHelper.IsValidImage(file))
            throw new InvalidOperationException("Geçersiz dosya formatı veya boyutu (max 5MB, jpg/png/webp).");

        var maxW = _config.GetValue("AppSettings:ProductImageMaxWidth", 800);
        var maxH = _config.GetValue("AppSettings:ProductImageMaxHeight", 800);
        var data = await ResizeAsync(file, maxW, maxH);

        var hasMain = await _db.ProductImages.AnyAsync(i => i.ProductId == productId && i.IsMain);
        var order   = await _db.ProductImages.CountAsync(i => i.ProductId == productId);

        var image = new ProductImage
        {
            ProductId    = productId,
            ImageData    = data,
            ContentType  = GetContentType(file.FileName),
            IsMain       = !hasMain,   // ilk resim her zaman main
            DisplayOrder = order + 1,
            ImageUrl     = string.Empty
        };

        _db.ProductImages.Add(image);
        await _db.SaveChangesAsync();                      // Id atanır

        image.ImageUrl = $"/Image/Get/{image.Id}";
        await _db.SaveChangesAsync();                      // Url güncellenir

        return image.ImageUrl;
    }

    public async Task<string> SaveProductThumbnailAsync(IFormFile file, int productId)
        => await SaveProductImageAsync(file, productId);  // aynı akış

    public void DeleteImage(string relativePath) { }      // DB'den silme repo ile yapılır

    private static async Task<byte[]> ResizeAsync(IFormFile file, int maxW, int maxH)
    {
        using var img = await Image.LoadAsync(file.OpenReadStream());
        if (img.Width > maxW || img.Height > maxH)
            img.Mutate(x => x.Resize(new ResizeOptions { Size = new Size(maxW, maxH), Mode = ResizeMode.Max }));
        using var ms = new MemoryStream();
        await img.SaveAsJpegAsync(ms);
        return ms.ToArray();
    }

    private static string GetContentType(string fileName) =>
        Path.GetExtension(fileName).ToLowerInvariant() switch
        {
            ".png"  => "image/png",
            ".webp" => "image/webp",
            ".gif"  => "image/gif",
            _       => "image/jpeg"
        };
}
