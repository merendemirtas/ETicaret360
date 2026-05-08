using ECommerceApp.Models.ViewModels;
using ECommerceApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerceApp.Controllers;

[Authorize]
public class CartController : Controller
{
    private readonly ICartService _cartService;
    private readonly ICouponService _couponService;
    private readonly IConfiguration _config;

    public CartController(ICartService cartService, ICouponService couponService, IConfiguration config)
    {
        _cartService = cartService;
        _couponService = couponService;
        _config = config;
    }

    [Route("sepet")]
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var items = await _cartService.GetCartItemsAsync(userId);

        var couponCode = HttpContext.Session.GetString("CouponCode");
        var couponId = HttpContext.Session.GetInt32("CouponId");
        decimal discount = 0;

        var subtotal = items.Sum(i => (i.Product.DiscountedPrice ?? i.Product.Price) * i.Quantity);

        if (couponId.HasValue)
            discount = await CalcDiscountAsync(couponId.Value, subtotal);

        var freeShip = _config.GetValue<decimal>("AppSettings:FreeShippingThreshold", 300);
        var shipCost = _config.GetValue<decimal>("AppSettings:ShippingCost", 29.90m);
        var shipping = (subtotal - discount) >= freeShip ? 0 : shipCost;

        var vm = new CartViewModel
        {
            Items = items.Select(i => new CartItemViewModel
            {
                Id           = i.Id,
                ProductId    = i.ProductId,
                ProductName  = i.Product.Name,
                ProductSlug  = i.Product.Slug,
                ImageUrl     = i.Product.Images?.FirstOrDefault(x => x.IsMain)?.ImageUrl ?? "/images/no-image.svg",
                UnitPrice    = i.Product.DiscountedPrice ?? i.Product.Price,
                Quantity     = i.Quantity,
                TotalPrice   = (i.Product.DiscountedPrice ?? i.Product.Price) * i.Quantity,
                Stock        = i.Product.Stock
            }).ToList(),
            SubTotal         = subtotal,
            Discount         = discount,
            Shipping         = shipping,
            Total            = subtotal - discount + shipping,
            AppliedCouponCode = couponCode,
            AppliedCouponId  = couponId
        };

        return View("~/Views/Cart/Index.cshtml", vm);
    }

    [HttpGet]
    [Route("sepet/count")]
    [AllowAnonymous]
    public async Task<IActionResult> Count()
    {
        if (!User.Identity?.IsAuthenticated == true)
            return Json(new { count = 0 });
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var count = await _cartService.GetCartCountAsync(userId);
        return Json(new { count });
    }

    [HttpPost]
    [Route("Cart/Add")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Add(int productId, int quantity = 1)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _cartService.AddToCartAsync(userId, productId, quantity);
            var count = await _cartService.GetCartCountAsync(userId);
            return Json(new { success = true, message = "Ürün sepete eklendi.", cartCount = count });
        }
        catch (InvalidOperationException ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [Route("Cart/UpdateQuantity")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _cartService.UpdateQuantityAsync(userId, cartItemId, quantity);
        return Json(new { success = true });
    }

    [HttpPost]
    [Route("Cart/Remove")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Remove(int cartItemId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _cartService.RemoveFromCartAsync(userId, cartItemId);
        return Json(new { success = true, message = "Ürün sepetten çıkarıldı." });
    }

    [HttpPost]
    [Route("Cart/ApplyCoupon")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> ApplyCoupon(string couponCode, decimal orderAmount)
    {
        var (isValid, message, _) = await _couponService.ValidateCouponAsync(couponCode, orderAmount);
        if (isValid)
        {
            var coupon = await _couponService.GetByCodeAsync(couponCode);
            HttpContext.Session.SetInt32("CouponId", coupon!.Id);
            HttpContext.Session.SetString("CouponCode", couponCode.ToUpper());
        }
        return Json(new { success = isValid, message });
    }

    [HttpPost]
    [Route("Cart/RemoveCoupon")]
    [IgnoreAntiforgeryToken]
    public IActionResult RemoveCoupon()
    {
        HttpContext.Session.Remove("CouponId");
        HttpContext.Session.Remove("CouponCode");
        return Json(new { success = true });
    }

    private async Task<decimal> CalcDiscountAsync(int couponId, decimal subtotal)
    {
        var coupon = await _couponService.GetByIdAsync(couponId);
        if (coupon is null) return 0;
        var d = coupon.DiscountType == ECommerceApp.Models.Entities.DiscountType.Percentage
            ? subtotal * coupon.DiscountValue / 100
            : coupon.DiscountValue;
        return coupon.MaxDiscountAmount.HasValue ? Math.Min(d, coupon.MaxDiscountAmount.Value) : d;
    }
}
