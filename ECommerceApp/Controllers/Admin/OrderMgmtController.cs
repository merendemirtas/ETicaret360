using ECommerceApp.Helpers;
using ECommerceApp.Models.Entities;
using ECommerceApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApp.Controllers.Admin;

[Authorize(Roles = Constants.AdminRole)]
[Route("admin/siparisler/[action]")]
public class OrderMgmtController : Controller
{
    private readonly IOrderService _orderService;

    public OrderMgmtController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [Route("/admin/siparisler")]
    public async Task<IActionResult> Index(OrderStatus? durum)
    {
        var orders = await _orderService.GetAllOrdersAsync(durum);
        ViewBag.CurrentStatus = durum;
        return View("~/Views/Admin/OrderMgmt/Index.cshtml", orders);
    }

    [Route("/admin/siparisler/detay/{id}")]
    public async Task<IActionResult> Detail(int id)
    {
        var order = await _orderService.GetByIdAsync(id);
        if (order is null) return NotFound();
        return View("~/Views/Admin/OrderMgmt/Detail.cshtml", order);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, OrderStatus status)
    {
        await _orderService.UpdateStatusAsync(id, status);
        TempData["Success"] = "Sipariş durumu güncellendi.";
        return RedirectToAction(nameof(Detail), new { id });
    }
}
