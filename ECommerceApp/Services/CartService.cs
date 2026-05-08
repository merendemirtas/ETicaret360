using ECommerceApp.Data.Repositories;
using ECommerceApp.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Services;

public class CartService : ICartService
{
    private readonly IRepository<CartItem> _cartRepo;
    private readonly IRepository<Product> _productRepo;

    public CartService(IRepository<CartItem> cartRepo, IRepository<Product> productRepo)
    {
        _cartRepo = cartRepo;
        _productRepo = productRepo;
    }

    public async Task<IEnumerable<CartItem>> GetCartItemsAsync(string userId) =>
        await _cartRepo.Query()
            .Include(c => c.Product).ThenInclude(p => p.Images)
            .Where(c => c.UserId == userId)
            .ToListAsync();

    public async Task<int> GetCartCountAsync(string userId) =>
        await _cartRepo.CountAsync(c => c.UserId == userId);

    public async Task AddToCartAsync(string userId, int productId, int quantity = 1)
    {
        var product = await _productRepo.GetByIdAsync(productId)
            ?? throw new InvalidOperationException("Ürün bulunamadı.");

        if (product.Stock < quantity)
            throw new InvalidOperationException("Yeterli stok yok.");

        var existing = (await _cartRepo.FindAsync(c => c.UserId == userId && c.ProductId == productId)).FirstOrDefault();
        if (existing is not null)
        {
            existing.Quantity += quantity;
            await _cartRepo.UpdateAsync(existing);
        }
        else
        {
            await _cartRepo.AddAsync(new CartItem
            {
                UserId = userId,
                ProductId = productId,
                Quantity = quantity,
                AddedAt = DateTime.UtcNow
            });
        }
    }

    public async Task UpdateQuantityAsync(string userId, int cartItemId, int quantity)
    {
        var item = (await _cartRepo.FindAsync(c => c.Id == cartItemId && c.UserId == userId)).FirstOrDefault();
        if (item is null) return;

        if (quantity <= 0)
        {
            await _cartRepo.DeleteAsync(item);
            return;
        }

        item.Quantity = quantity;
        await _cartRepo.UpdateAsync(item);
    }

    public async Task RemoveFromCartAsync(string userId, int cartItemId)
    {
        var item = (await _cartRepo.FindAsync(c => c.Id == cartItemId && c.UserId == userId)).FirstOrDefault();
        if (item is not null)
            await _cartRepo.DeleteAsync(item);
    }

    public async Task ClearCartAsync(string userId)
    {
        var items = await _cartRepo.FindAsync(c => c.UserId == userId);
        foreach (var item in items)
            await _cartRepo.DeleteAsync(item);
    }
}
