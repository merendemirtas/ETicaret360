using ECommerceApp.Helpers;
using ECommerceApp.Models.Entities;
using ECommerceApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApp.Controllers.Admin;

[Authorize(Roles = Constants.AdminRole)]
[Route("admin/markalar/[action]")]
public class BrandController : Controller
{
    private readonly IBrandService _brandService;
    private readonly IImageService _imageService;

    public BrandController(IBrandService brandService, IImageService imageService)
    {
        _brandService = brandService;
        _imageService = imageService;
    }

    [Route("/admin/markalar")]
    public async Task<IActionResult> Index()
    {
        var brands = await _brandService.GetAllAsync();
        return View("~/Views/Admin/Brand/Index.cshtml", brands);
    }

    [HttpGet]
    public IActionResult Create() => View("~/Views/Admin/Brand/Create.cshtml");

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Brand model, IFormFile? logoFile)
    {
        if (!ModelState.IsValid) return View("~/Views/Admin/Brand/Create.cshtml", model);

        await _brandService.CreateAsync(model);

        if (logoFile is not null && FileHelper.IsValidImage(logoFile))
        {
            var logoUrl = await _imageService.SaveProductImageAsync(logoFile, model.Id);
            model.LogoUrl = logoUrl;
            await _brandService.UpdateAsync(model);
        }

        TempData["Success"] = "Marka oluşturuldu.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Route("/admin/markalar/duzenle/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var brand = await _brandService.GetByIdAsync(id);
        if (brand is null) return NotFound();
        return View("~/Views/Admin/Brand/Edit.cshtml", brand);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("/admin/markalar/duzenle/{id}")]
    public async Task<IActionResult> Edit(int id, Brand model, IFormFile? logoFile)
    {
        if (!ModelState.IsValid) return View("~/Views/Admin/Brand/Edit.cshtml", model);

        if (logoFile is not null && FileHelper.IsValidImage(logoFile))
        {
            var logoUrl = await _imageService.SaveProductImageAsync(logoFile, model.Id);
            model.LogoUrl = logoUrl;
        }

        await _brandService.UpdateAsync(model);
        TempData["Success"] = "Marka güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _brandService.DeleteAsync(id);
        TempData["Success"] = "Marka pasif yapıldı.";
        return RedirectToAction(nameof(Index));
    }
}
