using ECommerceApp.Data.Repositories;
using ECommerceApp.Helpers;
using ECommerceApp.Models.Entities;
using ECommerceApp.Models.ViewModels;
using ECommerceApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace ECommerceApp.Controllers.Admin;

[Authorize(Roles = Constants.AdminRole)]
[Route("admin/urunler/[action]")]
public class ProductMgmtController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly IBrandService _brandService;
    private readonly IImageService _imageService;
    private readonly IRepository<ProductImage> _imageRepo;

    public ProductMgmtController(IProductService productService, ICategoryService categoryService,
        IBrandService brandService, IImageService imageService, IRepository<ProductImage> imageRepo)
    {
        _productService = productService;
        _categoryService = categoryService;
        _brandService = brandService;
        _imageService = imageService;
        _imageRepo = imageRepo;
    }

    [Route("/admin/urunler")]
    public async Task<IActionResult> Index(string? search, int sayfa = 1)
    {
        var products = await _productService.GetProductsAsync(sayfa, Constants.AdminPageSize, sortBy: "new");
        ViewBag.Search = search;
        return View("~/Views/Admin/ProductMgmt/Index.cshtml", products);
    }

    [HttpGet]
    [Route("/admin/urunler/ekle")]
    public async Task<IActionResult> Create()
    {
        var vm = new ProductCreateEditViewModel
        {
            Categories = await _categoryService.GetAllActiveAsync(),
            Brands     = await _brandService.GetAllActiveAsync(),
            IsActive   = true
        };
        return View("~/Views/Admin/ProductMgmt/Create.cshtml", vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("/admin/urunler/ekle")]
    public async Task<IActionResult> CreatePost()
    {
        var f = Request.Form;

        // Form değerlerini güvenli şekilde oku
        var name        = f["Name"].ToString().Trim();
        var description = f["Description"].ToString().Trim();
        var shortDesc   = f["ShortDescription"].ToString().Trim();
        var priceStr    = f["Price"].ToString().Replace(',', '.');
        var discStr     = f["DiscountedPrice"].ToString().Replace(',', '.');
        var stockStr    = f["Stock"].ToString();

        decimal.TryParse(priceStr,  NumberStyles.Any, CultureInfo.InvariantCulture, out var price);
        decimal.TryParse(discStr,   NumberStyles.Any, CultureInfo.InvariantCulture, out var discountedPrice);
        int.TryParse(stockStr,      out var stock);
        int.TryParse(f["CategoryId"], out var categoryId);
        int.TryParse(f["BrandId"],    out var brandId);
        var isActive   = f["IsActive"].Contains("true");
        var isFeatured = f["IsFeatured"].Contains("true");

        // Basit validasyon
        if (string.IsNullOrEmpty(name))   { TempData["Error"] = "Ürün adı zorunludur."; return await ReturnCreateView(); }
        if (price <= 0)                   { TempData["Error"] = "Geçerli bir fiyat girin."; return await ReturnCreateView(); }
        if (categoryId == 0)              { TempData["Error"] = "Kategori seçiniz."; return await ReturnCreateView(); }
        if (brandId == 0)                 { TempData["Error"] = "Marka seçiniz."; return await ReturnCreateView(); }

        var product = new Product
        {
            Name             = name,
            Description      = string.IsNullOrEmpty(description) ? name : description,
            ShortDescription = string.IsNullOrEmpty(shortDesc) ? null : shortDesc,
            Price            = price,
            DiscountedPrice  = discountedPrice > 0 && discountedPrice < price ? discountedPrice : null,
            Stock            = stock,
            CategoryId       = categoryId,
            BrandId          = brandId,
            IsActive         = isActive,
            IsFeatured       = isFeatured
        };

        await _productService.CreateAsync(product);

        // Resimleri kaydet — ilki otomatik main olacak (ImageService halleder)
        var files = Request.Form.Files.Where(f => f.Length > 0).ToList();
        foreach (var file in files)
        {
            try { await _imageService.SaveProductImageAsync(file, product.Id); }
            catch (Exception ex) { TempData["Error"] = $"Resim yüklenemedi: {ex.Message}"; }
        }

        TempData["Success"] = $"'{product.Name}' başarıyla eklendi.";
        return Redirect("/admin/urunler");
    }

    [HttpGet]
    [Route("/admin/urunler/duzenle/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var product = await GetProductWithImages(id);
        if (product is null) return NotFound();

        var vm = new ProductCreateEditViewModel
        {
            Id               = product.Id,
            Name             = product.Name,
            Description      = product.Description,
            ShortDescription = product.ShortDescription,
            Price            = product.Price,
            DiscountedPrice  = product.DiscountedPrice,
            Stock            = product.Stock,
            CategoryId       = product.CategoryId,
            BrandId          = product.BrandId,
            IsActive         = product.IsActive,
            IsFeatured       = product.IsFeatured,
            Slug             = product.Slug,
            ExistingImages   = product.Images.ToList(),
            Categories       = await _categoryService.GetAllActiveAsync(),
            Brands           = await _brandService.GetAllActiveAsync()
        };

        return View("~/Views/Admin/ProductMgmt/Edit.cshtml", vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("/admin/urunler/duzenle/{id}")]
    public async Task<IActionResult> EditPost(int id)
    {
        var product = await GetProductWithImages(id);
        if (product is null) return NotFound();

        var f = Request.Form;

        var priceStr = f["Price"].ToString().Replace(',', '.');
        var discStr  = f["DiscountedPrice"].ToString().Replace(',', '.');

        decimal.TryParse(priceStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var price);
        decimal.TryParse(discStr,  NumberStyles.Any, CultureInfo.InvariantCulture, out var discountedPrice);
        int.TryParse(f["Stock"],      out var stock);
        int.TryParse(f["CategoryId"], out var categoryId);
        int.TryParse(f["BrandId"],    out var brandId);

        product.Name             = f["Name"].ToString().Trim();
        product.Description      = f["Description"].ToString().Trim();
        product.ShortDescription = f["ShortDescription"].ToString().Trim().NullIfEmpty();
        product.Price            = price > 0 ? price : product.Price;
        product.DiscountedPrice  = discountedPrice > 0 && discountedPrice < price ? discountedPrice : null;
        product.Stock            = stock;
        product.CategoryId       = categoryId > 0 ? categoryId : product.CategoryId;
        product.BrandId          = brandId > 0 ? brandId : product.BrandId;
        product.IsActive         = f["IsActive"].Contains("true");
        product.IsFeatured       = f["IsFeatured"].Contains("true");

        await _productService.UpdateAsync(product);

        foreach (var file in Request.Form.Files.Where(x => x.Length > 0))
        {
            try { await _imageService.SaveProductImageAsync(file, product.Id); }
            catch (Exception ex) { TempData["Error"] = $"Resim yüklenemedi: {ex.Message}"; }
        }

        TempData["Success"] = "Ürün başarıyla güncellendi.";
        return Redirect("/admin/urunler");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _productService.DeleteAsync(id);
        TempData["Success"] = "Ürün pasif yapıldı.";
        return Redirect("/admin/urunler");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteImage(int imageId, int productId)
    {
        var image = await _imageRepo.GetByIdAsync(imageId);
        if (image is not null)
            await _imageRepo.DeleteAsync(image);
        return Redirect($"/admin/urunler/duzenle/{productId}");
    }

    private async Task<IActionResult> ReturnCreateView()
    {
        var vm = new ProductCreateEditViewModel
        {
            Categories = await _categoryService.GetAllActiveAsync(),
            Brands     = await _brandService.GetAllActiveAsync()
        };
        return View("~/Views/Admin/ProductMgmt/Create.cshtml", vm);
    }

    private async Task<Product?> GetProductWithImages(int id) =>
        await _productService.GetByIdAsync(id) ??
        await _imageRepo.Query()
            .Include(i => i.Product).ThenInclude(p => p.Images)
            .Where(i => i.ProductId == id)
            .Select(i => i.Product)
            .FirstOrDefaultAsync();
}

// Extension method
file static class StringExtensions
{
    public static string? NullIfEmpty(this string? s) =>
        string.IsNullOrWhiteSpace(s) ? null : s;
}
