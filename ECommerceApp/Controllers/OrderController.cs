using ECommerceApp.Data.Repositories;
using ECommerceApp.Models.Entities;
using ECommerceApp.Models.ViewModels;
using ECommerceApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerceApp.Controllers;

[Authorize]
public class OrderController : Controller
{
    private readonly IOrderService _orderService;
    private readonly ICartService _cartService;
    private readonly IRepository<Address> _addressRepo;
    private readonly IEmailService _emailService;

    public OrderController(IOrderService orderService, ICartService cartService,
        IRepository<Address> addressRepo, IEmailService emailService)
    {
        _orderService = orderService;
        _cartService = cartService;
        _addressRepo = addressRepo;
        _emailService = emailService;
    }

    [Route("sepet/odeme")]
    public async Task<IActionResult> Checkout()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var cartItems = await _cartService.GetCartItemsAsync(userId);
        if (!cartItems.Any())
        {
            TempData["Error"] = "Sepetiniz boş.";
            return Redirect("/sepet");
        }

        var couponId = HttpContext.Session.GetInt32("CouponId");
        var couponCode = HttpContext.Session.GetString("CouponCode");
        var (subtotal, discount, shipping, total) = await _orderService.CalculateTotalsAsync(userId, couponId);
        var addresses = (await _addressRepo.FindAsync(a => a.UserId == userId)).ToList();

        var vm = new CheckoutViewModel
        {
            Addresses      = addresses,
            SelectedAddressId = addresses.FirstOrDefault(a => a.IsDefault)?.Id,
            SubTotal       = subtotal,
            Discount       = discount,
            Shipping       = shipping,
            Total          = total,
            CouponId       = couponId,
            CouponCode     = couponCode,
            PaymentProvider = "COD"   // varsayılan: kapıda ödeme
        };

        return View("~/Views/Order/Checkout.cshtml", vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("sepet/odeme")]
    public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        int addressId = model.SelectedAddressId ?? 0;

        // Yeni adres ekle
        if (addressId == 0 && model.NewAddress is not null
            && !string.IsNullOrWhiteSpace(model.NewAddress.FullAddress))
        {
            var addr = new Address
            {
                UserId      = userId,
                Title       = model.NewAddress.Title ?? "Adresim",
                FullName    = model.NewAddress.FullName,
                Phone       = model.NewAddress.Phone,
                City        = model.NewAddress.City,
                District    = model.NewAddress.District,
                FullAddress = model.NewAddress.FullAddress,
                PostalCode  = model.NewAddress.PostalCode ?? "",
                IsDefault   = model.NewAddress.IsDefault
            };
            await _addressRepo.AddAsync(addr);
            addressId = addr.Id;
        }

        if (addressId == 0)
        {
            TempData["Error"] = "Lütfen bir teslimat adresi seçin veya yeni adres ekleyin.";
            return Redirect("/sepet/odeme");
        }

        try
        {
            var couponId = HttpContext.Session.GetInt32("CouponId");

            // Ödeme sağlayıcı: prototip — her zaman "COD" (Kapıda Ödeme)
            var order = await _orderService.CreateOrderAsync(
                userId, addressId, "COD", couponId, model.Note);

            HttpContext.Session.Remove("CouponId");
            HttpContext.Session.Remove("CouponCode");

            // Mail göndermeyi dene ama başarısız olursa siparişi etkileme
            try
            {
                var email = User.FindFirstValue(ClaimTypes.Email) ?? "";
                var name  = User.FindFirstValue(ClaimTypes.Name) ?? "Müşteri";
                if (!string.IsNullOrEmpty(email))
                {
                    var items = string.Join("", order.Items.Select(i =>
                        $"<li>{i.ProductName} × {i.Quantity} — {i.TotalPrice:N0} ₺</li>"));
                    await _emailService.SendOrderConfirmationAsync(
                        email, name, order.OrderNumber, order.TotalAmount, $"<ul>{items}</ul>");
                }
            }
            catch { /* SMTP yapılandırılmamış, sorun değil */ }

            return RedirectToAction(nameof(ThankYou), new { orderNumber = order.OrderNumber });
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
            return Redirect("/sepet/odeme");
        }
    }

    [Route("siparis/tesekkur/{orderNumber}")]
    public async Task<IActionResult> ThankYou(string orderNumber)
    {
        var order = await _orderService.GetByOrderNumberAsync(orderNumber);
        if (order is null) return NotFound();
        if (order.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier)) return Forbid();
        return View("~/Views/Order/ThankYou.cshtml", order);
    }

    [Route("siparis/siparislerim")]
    public async Task<IActionResult> MyOrders()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var orders = await _orderService.GetUserOrdersAsync(userId);
        return View("~/Views/Order/MyOrders.cshtml", orders);
    }

    [Route("siparis/detay/{id}")]
    public async Task<IActionResult> Detail(int id)
    {
        var order = await _orderService.GetByIdAsync(id);
        if (order is null) return NotFound();
        if (order.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier)) return Forbid();
        return View("~/Views/Order/Detail.cshtml", new OrderDetailViewModel { Order = order });
    }
}
