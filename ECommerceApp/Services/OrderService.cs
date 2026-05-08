using ECommerceApp.Data.Repositories;
using ECommerceApp.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Services;

public class OrderService : IOrderService
{
    private readonly IRepository<Order> _orderRepo;
    private readonly IRepository<CartItem> _cartRepo;
    private readonly IRepository<Coupon> _couponRepo;
    private readonly IRepository<Product> _productRepo;
    private readonly IConfiguration _config;

    public OrderService(IRepository<Order> orderRepo, IRepository<CartItem> cartRepo,
        IRepository<Coupon> couponRepo, IRepository<Product> productRepo, IConfiguration config)
    {
        _orderRepo = orderRepo;
        _cartRepo = cartRepo;
        _couponRepo = couponRepo;
        _productRepo = productRepo;
        _config = config;
    }

    public async Task<Order?> GetByIdAsync(int id) =>
        await _orderRepo.Query()
            .Include(o => o.Items)
            .Include(o => o.Address)
            .Include(o => o.Payment)
            .Include(o => o.Coupon)
            .FirstOrDefaultAsync(o => o.Id == id);

    public async Task<Order?> GetByOrderNumberAsync(string orderNumber) =>
        await _orderRepo.Query()
            .Include(o => o.Items)
            .Include(o => o.Address)
            .Include(o => o.Payment)
            .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);

    public async Task<IEnumerable<Order>> GetUserOrdersAsync(string userId) =>
        await _orderRepo.Query()
            .Include(o => o.Items)
            .Include(o => o.Payment)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderedAt)
            .ToListAsync();

    public async Task<IEnumerable<Order>> GetAllOrdersAsync(OrderStatus? status = null)
    {
        var query = _orderRepo.Query()
            .Include(o => o.User)
            .Include(o => o.Items)
            .AsQueryable();

        if (status.HasValue)
            query = query.Where(o => o.Status == status.Value);

        return await query.OrderByDescending(o => o.OrderedAt).ToListAsync();
    }

    public async Task<Order> CreateOrderAsync(string userId, int addressId, string paymentProvider, int? couponId, string? note)
    {
        var cartItems = await _cartRepo.Query()
            .Include(c => c.Product).ThenInclude(p => p.Images)
            .Where(c => c.UserId == userId)
            .ToListAsync();

        if (!cartItems.Any())
            throw new InvalidOperationException("Sepet boş.");

        var (subtotal, discount, shipping, total) = await CalculateTotalsAsync(userId, couponId);

        var orderNumber = GenerateOrderNumber();

        var order = new Order
        {
            OrderNumber = orderNumber,
            UserId = userId,
            AddressId = addressId,
            CouponId = couponId,
            SubTotal = subtotal,
            DiscountAmount = discount,
            ShippingCost = shipping,
            TotalAmount = total,
            Status = OrderStatus.Pending,
            Note = note,
            OrderedAt = DateTime.UtcNow,
            Items = cartItems.Select(c => new OrderItem
            {
                ProductId = c.ProductId,
                ProductName = c.Product.Name,
                ProductImageUrl = c.Product.Images?.FirstOrDefault(i => i.IsMain)?.ImageUrl ?? "/images/no-image.png",
                Quantity = c.Quantity,
                UnitPrice = c.Product.DiscountedPrice ?? c.Product.Price,
                TotalPrice = (c.Product.DiscountedPrice ?? c.Product.Price) * c.Quantity
            }).ToList(),
            Payment = new Payment
            {
                Provider = paymentProvider,
                Amount = total,
                Status = paymentProvider == "COD" ? PaymentStatus.Pending : PaymentStatus.Completed,
                PaidAt = paymentProvider != "COD" ? DateTime.UtcNow : null,
                CreatedAt = DateTime.UtcNow
            }
        };

        await _orderRepo.AddAsync(order);

        foreach (var item in cartItems)
        {
            var product = await _productRepo.GetByIdAsync(item.ProductId);
            if (product is not null)
            {
                product.Stock = Math.Max(0, product.Stock - item.Quantity);
                await _productRepo.UpdateAsync(product);
            }
        }

        if (couponId.HasValue)
        {
            var coupon = await _couponRepo.GetByIdAsync(couponId.Value);
            if (coupon is not null)
            {
                coupon.UsedCount++;
                await _couponRepo.UpdateAsync(coupon);
            }
        }

        foreach (var item in cartItems)
            await _cartRepo.DeleteAsync(item);

        return order;
    }

    public async Task UpdateStatusAsync(int orderId, OrderStatus status)
    {
        var order = await _orderRepo.GetByIdAsync(orderId);
        if (order is not null)
        {
            order.Status = status;
            await _orderRepo.UpdateAsync(order);
        }
    }

    public async Task<(decimal subtotal, decimal discount, decimal shipping, decimal total)> CalculateTotalsAsync(string userId, int? couponId)
    {
        var cartItems = await _cartRepo.Query()
            .Include(c => c.Product)
            .Where(c => c.UserId == userId)
            .ToListAsync();

        var subtotal = cartItems.Sum(c => (c.Product.DiscountedPrice ?? c.Product.Price) * c.Quantity);
        var freeShippingThreshold = _config.GetValue<decimal>("AppSettings:FreeShippingThreshold", 300);
        var shippingCost = _config.GetValue<decimal>("AppSettings:ShippingCost", 29.90m);

        decimal discount = 0;
        if (couponId.HasValue)
        {
            var coupon = await _couponRepo.GetByIdAsync(couponId.Value);
            if (coupon is not null && coupon.IsActive && (coupon.ExpiresAt is null || coupon.ExpiresAt > DateTime.UtcNow))
            {
                if (coupon.MinOrderAmount is null || subtotal >= coupon.MinOrderAmount)
                {
                    discount = coupon.DiscountType == DiscountType.Percentage
                        ? subtotal * coupon.DiscountValue / 100
                        : coupon.DiscountValue;

                    if (coupon.MaxDiscountAmount.HasValue)
                        discount = Math.Min(discount, coupon.MaxDiscountAmount.Value);
                }
            }
        }

        var shipping = (subtotal - discount) >= freeShippingThreshold ? 0 : shippingCost;
        var total = subtotal - discount + shipping;

        return (subtotal, discount, shipping, total);
    }

    private static string GenerateOrderNumber()
    {
        var date = DateTime.UtcNow.ToString("yyyyMMdd");
        var random = new Random().Next(1000, 9999);
        return $"ORD-{date}-{random}";
    }
}
