using ECommerceApp.Data.Repositories;
using ECommerceApp.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Services;

public class ReviewService : IReviewService
{
    private readonly IRepository<Review> _reviewRepo;
    private readonly IRepository<OrderItem> _orderItemRepo;

    public ReviewService(IRepository<Review> reviewRepo, IRepository<OrderItem> orderItemRepo)
    {
        _reviewRepo = reviewRepo;
        _orderItemRepo = orderItemRepo;
    }

    public async Task<IEnumerable<Review>> GetProductReviewsAsync(int productId) =>
        await _reviewRepo.Query()
            .Include(r => r.User)
            .Where(r => r.ProductId == productId && r.IsApproved)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

    public async Task<IEnumerable<Review>> GetPendingReviewsAsync() =>
        await _reviewRepo.Query()
            .Include(r => r.User)
            .Include(r => r.Product)
            .Where(r => !r.IsApproved)
            .OrderBy(r => r.CreatedAt)
            .ToListAsync();

    public async Task<bool> HasUserPurchasedAsync(string userId, int productId) =>
        await _orderItemRepo.Query()
            .Include(i => i.Order)
            .AnyAsync(i => i.ProductId == productId && i.Order.UserId == userId &&
                           i.Order.Status == OrderStatus.Delivered);

    public async Task<bool> HasUserReviewedAsync(string userId, int productId) =>
        await _reviewRepo.ExistsAsync(r => r.UserId == userId && r.ProductId == productId);

    public async Task AddReviewAsync(Review review)
    {
        review.CreatedAt = DateTime.UtcNow;
        review.IsApproved = false;
        await _reviewRepo.AddAsync(review);
    }

    public async Task ApproveReviewAsync(int reviewId)
    {
        var review = await _reviewRepo.GetByIdAsync(reviewId);
        if (review is not null)
        {
            review.IsApproved = true;
            await _reviewRepo.UpdateAsync(review);
        }
    }

    public async Task DeleteReviewAsync(int reviewId)
    {
        var review = await _reviewRepo.GetByIdAsync(reviewId);
        if (review is not null)
            await _reviewRepo.DeleteAsync(review);
    }
}
