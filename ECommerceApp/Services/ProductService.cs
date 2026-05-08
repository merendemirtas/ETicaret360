using ECommerceApp.Data.Repositories;
using ECommerceApp.Helpers;
using ECommerceApp.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Services;

public class ProductService : IProductService
{
    private readonly IRepository<Product> _repo;

    public ProductService(IRepository<Product> repo)
    {
        _repo = repo;
    }

    public async Task<PaginatedList<Product>> GetProductsAsync(int page, int pageSize, int? categoryId = null,
        int? brandId = null, decimal? minPrice = null, decimal? maxPrice = null,
        string? sortBy = null, string? searchQuery = null, bool? discounted = null)
    {
        var query = _repo.Query()
            .Include(p => p.Images)
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Reviews)
            .Where(p => p.IsActive);

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);

        if (brandId.HasValue)
            query = query.Where(p => p.BrandId == brandId.Value);

        if (minPrice.HasValue)
            query = query.Where(p => (p.DiscountedPrice ?? p.Price) >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(p => (p.DiscountedPrice ?? p.Price) <= maxPrice.Value);

        if (!string.IsNullOrWhiteSpace(searchQuery))
            query = query.Where(p => p.Name.Contains(searchQuery) ||
                                     p.Description.Contains(searchQuery) ||
                                     p.Brand.Name.Contains(searchQuery));

        if (discounted == true)
            query = query.Where(p => p.DiscountedPrice != null && p.DiscountedPrice < p.Price);

        query = sortBy switch
        {
            "price_asc"  => query.OrderBy(p => p.DiscountedPrice ?? p.Price),
            "price_desc" => query.OrderByDescending(p => p.DiscountedPrice ?? p.Price),
            "featured"   => query.OrderByDescending(p => p.IsFeatured).ThenByDescending(p => p.CreatedAt),
            "rating"     => query.OrderByDescending(p => p.Reviews.Where(r => r.IsApproved).Average(r => (double?)r.Rating) ?? 0),
            _            => query.OrderByDescending(p => p.CreatedAt)
        };

        return await PaginatedList<Product>.CreateAsync(query, page, pageSize);
    }

    public async Task<Product?> GetByIdAsync(int id) =>
        await _repo.Query()
            .Include(p => p.Images)
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Reviews.Where(r => r.IsApproved)).ThenInclude(r => r.User)
            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

    public async Task<Product?> GetBySlugAsync(string slug) =>
        await _repo.Query()
            .Include(p => p.Images)
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Reviews.Where(r => r.IsApproved)).ThenInclude(r => r.User)
            .FirstOrDefaultAsync(p => p.Slug == slug && p.IsActive);

    public async Task<IEnumerable<Product>> GetFeaturedAsync(int count = 8) =>
        await _repo.Query()
            .Include(p => p.Images)
            .Include(p => p.Brand)
            .Where(p => p.IsActive && p.IsFeatured)
            .OrderByDescending(p => p.CreatedAt)
            .Take(count)
            .ToListAsync();

    public async Task<IEnumerable<Product>> GetNewArrivalsAsync(int count = 8) =>
        await _repo.Query()
            .Include(p => p.Images)
            .Include(p => p.Brand)
            .Where(p => p.IsActive)
            .OrderByDescending(p => p.CreatedAt)
            .Take(count)
            .ToListAsync();

    public async Task<IEnumerable<Product>> GetDiscountedAsync(int count = 8) =>
        await _repo.Query()
            .Include(p => p.Images)
            .Include(p => p.Brand)
            .Where(p => p.IsActive && p.DiscountedPrice.HasValue)
            .OrderByDescending(p => p.CreatedAt)
            .Take(count)
            .ToListAsync();

    public async Task<IEnumerable<Product>> GetRelatedAsync(int productId, int categoryId, int count = 4) =>
        await _repo.Query()
            .Include(p => p.Images)
            .Where(p => p.IsActive && p.CategoryId == categoryId && p.Id != productId)
            .OrderByDescending(p => p.CreatedAt)
            .Take(count)
            .ToListAsync();

    public async Task<Product> CreateAsync(Product product)
    {
        product.Slug = await GenerateUniqueSlugAsync(SlugHelper.GenerateSlug(product.Name));
        product.CreatedAt = DateTime.UtcNow;
        await _repo.AddAsync(product);
        return product;
    }

    public async Task UpdateAsync(Product product)
    {
        product.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(product);
    }

    public async Task DeleteAsync(int id)
    {
        var product = await _repo.GetByIdAsync(id);
        if (product is not null)
        {
            product.IsActive = false;
            await _repo.UpdateAsync(product);
        }
    }

    public async Task<bool> SlugExistsAsync(string slug, int? excludeId = null) =>
        await _repo.ExistsAsync(p => p.Slug == slug && (!excludeId.HasValue || p.Id != excludeId.Value));

    public async Task DecreaseStockAsync(int productId, int quantity)
    {
        var product = await _repo.GetByIdAsync(productId);
        if (product is not null)
        {
            product.Stock = Math.Max(0, product.Stock - quantity);
            await _repo.UpdateAsync(product);
        }
    }

    private async Task<string> GenerateUniqueSlugAsync(string baseSlug)
    {
        var existing = (await _repo.GetAllAsync()).Select(p => p.Slug).ToList();
        return SlugHelper.MakeUnique(baseSlug, existing);
    }
}
