using ECommerceApp.Models.ViewModels;
using ECommerceApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApp.Controllers;

public class HomeController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;

    public HomeController(IProductService productService, ICategoryService categoryService)
    {
        _productService = productService;
        _categoryService = categoryService;
    }

    public async Task<IActionResult> Index()
    {
        var vm = new HomeViewModel
        {
            FeaturedProducts = await _productService.GetFeaturedAsync(8),
            NewArrivals = await _productService.GetNewArrivalsAsync(8),
            DiscountedProducts = await _productService.GetDiscountedAsync(8),
            MainCategories = await _categoryService.GetMainCategoriesAsync()
        };
        return View(vm);
    }

    public IActionResult Error(int? statusCode)
    {
        if (statusCode == 404)
            return View("Error404");
        return View("Error500");
    }
}
