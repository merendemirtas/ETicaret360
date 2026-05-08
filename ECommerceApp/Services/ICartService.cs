using ECommerceApp.Models.Entities;

namespace ECommerceApp.Services;

public interface ICartService
{
    Task<IEnumerable<CartItem>> GetCartItemsAsync(string userId);
    Task<int> GetCartCountAsync(string userId);
    Task AddToCartAsync(string userId, int productId, int quantity = 1);
    Task UpdateQuantityAsync(string userId, int cartItemId, int quantity);
    Task RemoveFromCartAsync(string userId, int cartItemId);
    Task ClearCartAsync(string userId);
}
