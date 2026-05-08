using ECommerceApp.Models.Entities;
using ECommerceApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerceApp.Controllers;

[Authorize]
public class ReviewController : Controller
{
    private readonly IReviewService _reviewService;

    public ReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int productId, int rating, string? title, string comment, string returnSlug)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        if (await _reviewService.HasUserReviewedAsync(userId, productId))
        {
            TempData["Error"] = "Bu ürün için zaten yorum yaptınız.";
            return RedirectToAction("Detail", "Product", new { slug = returnSlug });
        }

        if (rating < 1 || rating > 5)
        {
            TempData["Error"] = "Geçersiz puan.";
            return RedirectToAction("Detail", "Product", new { slug = returnSlug });
        }

        await _reviewService.AddReviewAsync(new Review
        {
            UserId = userId,
            ProductId = productId,
            Rating = rating,
            Title = title,
            Comment = comment
        });

        TempData["Success"] = "Yorumunuz inceleme için gönderildi.";
        return RedirectToAction("Detail", "Product", new { slug = returnSlug });
    }
}
