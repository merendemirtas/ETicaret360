using ECommerceApp.Helpers;
using ECommerceApp.Models.Entities;
using ECommerceApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApp.Controllers.Admin;

[Authorize(Roles = Constants.AdminRole)]
[Route("admin/kategoriler/[action]")]
public class CategoryController : Controller
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [Route("/admin/kategoriler")]
    public async Task<IActionResult> Index()
    {
        var categories = await _categoryService.GetAllActiveAsync();
        return View("~/Views/Admin/Category/Index.cshtml", categories);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.ParentCategories = await _categoryService.GetMainCategoriesAsync();
        return View("~/Views/Admin/Category/Create.cshtml");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Category model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.ParentCategories = await _categoryService.GetMainCategoriesAsync();
            return View("~/Views/Admin/Category/Create.cshtml", model);
        }
        await _categoryService.CreateAsync(model);
        TempData["Success"] = "Kategori oluşturuldu.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Route("/admin/kategoriler/duzenle/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        if (category is null) return NotFound();
        ViewBag.ParentCategories = await _categoryService.GetMainCategoriesAsync();
        return View("~/Views/Admin/Category/Edit.cshtml", category);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("/admin/kategoriler/duzenle/{id}")]
    public async Task<IActionResult> Edit(int id, Category model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.ParentCategories = await _categoryService.GetMainCategoriesAsync();
            return View("~/Views/Admin/Category/Edit.cshtml", model);
        }
        model.Slug = SlugHelper.GenerateSlug(model.Name);
        await _categoryService.UpdateAsync(model);
        TempData["Success"] = "Kategori güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _categoryService.DeleteAsync(id);
        TempData["Success"] = "Kategori pasif yapıldı.";
        return RedirectToAction(nameof(Index));
    }
}
