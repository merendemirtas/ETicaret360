using ECommerceApp.Models.Entities;

namespace ECommerceApp.Services;

public interface IReviewService
{
    Task<IEnumerable<Review>> GetProductReviewsAsync(int productId);
    Task<IEnumerable<Review>> GetPendingReviewsAsync();
    Task<bool> HasUserPurchasedAsync(string userId, int productId);
    Task<bool> HasUserReviewedAsync(string userId, int productId);
    Task AddReviewAsync(Review review);
    Task ApproveReviewAsync(int reviewId);
    Task DeleteReviewAsync(int reviewId);
}
