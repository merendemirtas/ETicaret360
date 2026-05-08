using ECommerceApp.Helpers;
using ECommerceApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApp.Controllers.Admin;

[Authorize(Roles = Constants.AdminRole)]
[Route("admin/yorumlar/[action]")]
public class ReviewMgmtController : Controller
{
    private readonly IReviewService _reviewService;

    public ReviewMgmtController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [Route("/admin/yorumlar")]
    public async Task<IActionResult> Index()
    {
        var reviews = await _reviewService.GetPendingReviewsAsync();
        return View("~/Views/Admin/ReviewMgmt/Index.cshtml", reviews);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int id)
    {
        await _reviewService.ApproveReviewAsync(id);
        TempData["Success"] = "Yorum onaylandı.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _reviewService.DeleteReviewAsync(id);
        TempData["Success"] = "Yorum silindi.";
        return RedirectToAction(nameof(Index));
    }
}
