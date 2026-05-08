using ECommerceApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerceApp.Controllers;

[Authorize]
public class WishlistController : Controller
{
    private readonly IWishlistService _wishlistService;
    private readonly ICartService _cartService;

    public WishlistController(IWishlistService wishlistService, ICartService cartService)
    {
        _wishlistService = wishlistService;
        _cartService = cartService;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var wishlist = await _wishlistService.GetUserWishlistAsync(userId);
        return View(wishlist);
    }

    [HttpPost]
    public async Task<IActionResult> Toggle(int productId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var isInWishlist = await _wishlistService.IsInWishlistAsync(userId, productId);

        if (isInWishlist)
        {
            await _wishlistService.RemoveFromWishlistAsync(userId, productId);
            return Json(new { success = true, message = "Favorilerden çıkarıldı.", inWishlist = false });
        }
        else
        {
            await _wishlistService.AddToWishlistAsync(userId, productId);
            return Json(new { success = true, message = "Favorilere eklendi.", inWishlist = true });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Remove(int productId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _wishlistService.RemoveFromWishlistAsync(userId, productId);
        TempData["Success"] = "Ürün favorilerden çıkarıldı.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> MoveToCart(int productId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        try
        {
            await _cartService.AddToCartAsync(userId, productId);
            await _wishlistService.RemoveFromWishlistAsync(userId, productId);
            TempData["Success"] = "Ürün sepete taşındı.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }
}
