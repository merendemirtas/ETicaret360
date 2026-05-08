using ECommerceApp.Helpers;
using ECommerceApp.Models.Entities;
using ECommerceApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApp.Controllers.Admin;

[Authorize(Roles = Constants.AdminRole)]
[Route("admin/kuponlar/[action]")]
public class CouponController : Controller
{
    private readonly ICouponService _couponService;

    public CouponController(ICouponService couponService)
    {
        _couponService = couponService;
    }

    [Route("/admin/kuponlar")]
    public async Task<IActionResult> Index()
    {
        var coupons = await _couponService.GetAllAsync();
        return View("~/Views/Admin/Coupon/Index.cshtml", coupons);
    }

    [HttpGet]
    public IActionResult Create() => View("~/Views/Admin/Coupon/Create.cshtml");

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Coupon model)
    {
        if (!ModelState.IsValid) return View("~/Views/Admin/Coupon/Create.cshtml", model);
        await _couponService.CreateAsync(model);
        TempData["Success"] = "Kupon oluşturuldu.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Route("/admin/kuponlar/duzenle/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var coupon = await _couponService.GetByIdAsync(id);
        if (coupon is null) return NotFound();
        return View("~/Views/Admin/Coupon/Edit.cshtml", coupon);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("/admin/kuponlar/duzenle/{id}")]
    public async Task<IActionResult> Edit(int id, Coupon model)
    {
        if (!ModelState.IsValid) return View("~/Views/Admin/Coupon/Edit.cshtml", model);
        await _couponService.UpdateAsync(model);
        TempData["Success"] = "Kupon güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _couponService.DeleteAsync(id);
        TempData["Success"] = "Kupon silindi.";
        return RedirectToAction(nameof(Index));
    }
}
