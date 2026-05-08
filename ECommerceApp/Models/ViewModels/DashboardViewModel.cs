using ECommerceApp.Models.Entities;

namespace ECommerceApp.Models.ViewModels;

public class DashboardViewModel
{
    public decimal TotalSales { get; set; }
    public int TotalOrders { get; set; }
    public int TotalProducts { get; set; }
    public int TotalUsers { get; set; }
    public int PendingOrders { get; set; }
    public int ShippedOrders { get; set; }
    public int DeliveredOrders { get; set; }
    public int CancelledOrders { get; set; }
    public decimal TodaySales { get; set; }
    public int TodayOrders { get; set; }
    public IEnumerable<Order> RecentOrders { get; set; } = new List<Order>();
    public IEnumerable<Product> LowStockProducts { get; set; } = new List<Product>();
    public IEnumerable<MonthlyStat> MonthlyStats { get; set; } = new List<MonthlyStat>();
}

public class MonthlyStat
{
    public string Month { get; set; } = string.Empty;
    public decimal Sales { get; set; }
    public int Orders { get; set; }
}
