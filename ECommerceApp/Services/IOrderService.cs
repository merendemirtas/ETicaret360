using ECommerceApp.Models.Entities;

namespace ECommerceApp.Services;

public interface IOrderService
{
    Task<Order?> GetByIdAsync(int id);
    Task<Order?> GetByOrderNumberAsync(string orderNumber);
    Task<IEnumerable<Order>> GetUserOrdersAsync(string userId);
    Task<IEnumerable<Order>> GetAllOrdersAsync(OrderStatus? status = null);
    Task<Order> CreateOrderAsync(string userId, int addressId, string paymentProvider, int? couponId, string? note);
    Task UpdateStatusAsync(int orderId, OrderStatus status);
    Task<(decimal subtotal, decimal discount, decimal shipping, decimal total)> CalculateTotalsAsync(string userId, int? couponId);
}
