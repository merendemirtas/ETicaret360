using ECommerceApp.Helpers;
using ECommerceApp.Models.ViewModels;
using ECommerceApp.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerceApp.Controllers;

public class ProductController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly IBrandService _brandService;
    private readonly IWishlistService _wishlistService;

    public ProductController(IProductService productService, ICategoryService categoryService,
        IBrandService brandService, IWishlistService wishlistService)
    {
        _productService = productService;
        _categoryService = categoryService;
        _brandService = brandService;
        _wishlistService = wishlistService;
    }

    [Route("urunler")]
    public async Task<IActionResult> Index(string? ara, int? kategori, int? marka,
        decimal? minFiyat, decimal? maxFiyat, string? siralama, bool? indirimli, int sayfa = 1)
    {
        var products = await _productService.GetProductsAsync(sayfa, Constants.PageSize,
            kategori, marka, minFiyat, maxFiyat, siralama, ara, indirimli == true ? true : null);
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var productCards = new List<ProductCardViewModel>();
        foreach (var p in products.Items)
        {
            productCards.Add(new ProductCardViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Slug = p.Slug,
                ShortDescription = p.ShortDescription,
                Price = p.Price,
                DiscountedPrice = p.DiscountedPrice,
                MainImageUrl = p.Images?.FirstOrDefault(i => i.IsMain)?.ImageUrl ?? Constants.NoImagePath,
                BrandName = p.Brand?.Name ?? "",
                Stock = p.Stock,
                AverageRating = p.Reviews?.Any() == true ? p.Reviews.Average(r => r.Rating) : 0,
                ReviewCount = p.Reviews?.Count ?? 0,
                IsInWishlist = userId != null && await _wishlistService.IsInWishlistAsync(userId, p.Id)
            });
        }

        var vm = new ProductListViewModel
        {
            Products = productCards,
            CurrentPage = products.PageIndex,
            TotalPages = products.TotalPages,
            TotalCount = products.TotalCount,
            CategoryId = kategori,
            BrandId = marka,
            MinPrice = minFiyat,
            MaxPrice = maxFiyat,
            SortBy = siralama,
            SearchQuery = ara,
            Categories = await _categoryService.GetAllActiveAsync(),
            Brands = await _brandService.GetAllActiveAsync()
        };

        return View("~/Views/Product/Index.cshtml", vm);
    }

    [Route("urun/{slug}")]
    public async Task<IActionResult> Detail(string slug)
    {
        var product = await _productService.GetBySlugAsync(slug);
        if (product is null) return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isInWishlist = userId != null && await _wishlistService.IsInWishlistAsync(userId, product.Id);

        var vm = new ProductDetailViewModel
        {
            Product = product,
            IsInWishlist = isInWishlist,
            RelatedProducts = await _productService.GetRelatedAsync(product.Id, product.CategoryId),
            AverageRating = product.Reviews?.Any(r => r.IsApproved) == true
                ? product.Reviews.Where(r => r.IsApproved).Average(r => r.Rating) : 0
        };

        return View("~/Views/Product/Detail.cshtml", vm);
    }
}
