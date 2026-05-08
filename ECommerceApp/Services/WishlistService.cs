using ECommerceApp.Data.Repositories;
using ECommerceApp.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Services;

public class WishlistService : IWishlistService
{
    private readonly IRepository<Wishlist> _repo;

    public WishlistService(IRepository<Wishlist> repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<Wishlist>> GetUserWishlistAsync(string userId) =>
        await _repo.Query()
            .Include(w => w.Product).ThenInclude(p => p.Images)
            .Include(w => w.Product).ThenInclude(p => p.Brand)
            .Where(w => w.UserId == userId)
            .OrderByDescending(w => w.AddedAt)
            .ToListAsync();

    public async Task<bool> IsInWishlistAsync(string userId, int productId) =>
        await _repo.ExistsAsync(w => w.UserId == userId && w.ProductId == productId);

    public async Task AddToWishlistAsync(string userId, int productId)
    {
        if (await IsInWishlistAsync(userId, productId)) return;

        await _repo.AddAsync(new Wishlist
        {
            UserId = userId,
            ProductId = productId,
            AddedAt = DateTime.UtcNow
        });
    }

    public async Task RemoveFromWishlistAsync(string userId, int productId)
    {
        var item = (await _repo.FindAsync(w => w.UserId == userId && w.ProductId == productId)).FirstOrDefault();
        if (item is not null)
            await _repo.DeleteAsync(item);
    }
}
