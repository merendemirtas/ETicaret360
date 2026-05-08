using ECommerceApp.Models.Entities;

namespace ECommerceApp.Services;

public interface IWishlistService
{
    Task<IEnumerable<Wishlist>> GetUserWishlistAsync(string userId);
    Task<bool> IsInWishlistAsync(string userId, int productId);
    Task AddToWishlistAsync(string userId, int productId);
    Task RemoveFromWishlistAsync(string userId, int productId);
}
