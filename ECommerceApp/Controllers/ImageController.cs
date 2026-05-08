using ECommerceApp.Data.Repositories;
using ECommerceApp.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApp.Controllers;

public class ImageController : Controller
{
    private readonly IRepository<ProductImage> _imageRepo;

    public ImageController(IRepository<ProductImage> imageRepo)
    {
        _imageRepo = imageRepo;
    }

    [ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Any)]
    public async Task<IActionResult> Get(int id)
    {
        var image = await _imageRepo.GetByIdAsync(id);

        if (image?.ImageData == null)
            return Redirect("/images/no-image.png");

        return File(image.ImageData, image.ContentType ?? "image/jpeg");
    }
}
