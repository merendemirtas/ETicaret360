using ECommerceApp.Data.Repositories;
using ECommerceApp.Helpers;
using ECommerceApp.Models.Entities;
using ECommerceApp.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Controllers.Admin;

[Authorize(Roles = Constants.AdminRole)]
[Route("admin/[controller]/[action]")]
public class DashboardController : Controller
{
    private readonly IRepository<Order> _orderRepo;
    private readonly IRepository<Product> _productRepo;
    private readonly UserManager<ApplicationUser> _userManager;

    public DashboardController(IRepository<Order> orderRepo,
        IRepository<Product> productRepo, UserManager<ApplicationUser> userManager)
    {
        _orderRepo = orderRepo;
        _productRepo = productRepo;
        _userManager = userManager;
    }

    [Route("/admin")]
    [Route("/admin/dashboard")]
    public async Task<IActionResult> Index()
    {
        var allOrders = await _orderRepo.Query()
            .Include(o => o.User)
            .OrderByDescending(o => o.OrderedAt)
            .ToListAsync();

        var today = DateTime.UtcNow.Date;

        // Son 6 ay istatistik
        var monthlyStats = Enumerable.Range(0, 6)
            .Select(i => DateTime.UtcNow.AddMonths(-i))
            .Select(d => new MonthlyStat
            {
                Month  = d.ToString("MMM"),
                Sales  = allOrders
                    .Where(o => o.OrderedAt.Year == d.Year && o.OrderedAt.Month == d.Month
                             && o.Status == OrderStatus.Delivered)
                    .Sum(o => o.TotalAmount),
                Orders = allOrders
                    .Count(o => o.OrderedAt.Year == d.Year && o.OrderedAt.Month == d.Month)
            })
            .Reverse()
            .ToList();

        var vm = new DashboardViewModel
        {
            TotalSales       = allOrders.Where(o => o.Status == OrderStatus.Delivered).Sum(o => o.TotalAmount),
            TotalOrders      = allOrders.Count,
            TotalProducts    = await _productRepo.CountAsync(p => p.IsActive),
            TotalUsers       = _userManager.Users.Count(),
            PendingOrders    = allOrders.Count(o => o.Status == OrderStatus.Pending),
            ShippedOrders    = allOrders.Count(o => o.Status == OrderStatus.Shipped),
            DeliveredOrders  = allOrders.Count(o => o.Status == OrderStatus.Delivered),
            CancelledOrders  = allOrders.Count(o => o.Status == OrderStatus.Cancelled),
            TodaySales       = allOrders.Where(o => o.OrderedAt.Date == today).Sum(o => o.TotalAmount),
            TodayOrders      = allOrders.Count(o => o.OrderedAt.Date == today),
            RecentOrders     = allOrders.Take(8),
            MonthlyStats     = monthlyStats,
            LowStockProducts = await _productRepo.Query()
                .Where(p => p.IsActive && p.Stock < 5)
                .OrderBy(p => p.Stock)
                .Take(8)
                .ToListAsync()
        };

        return View("~/Views/Admin/Dashboard/Index.cshtml", vm);
    }
}
