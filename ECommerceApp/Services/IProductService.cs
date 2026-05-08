using ECommerceApp.Helpers;
using ECommerceApp.Models.Entities;

namespace ECommerceApp.Services;

public interface IProductService
{
    Task<PaginatedList<Product>> GetProductsAsync(int page, int pageSize, int? categoryId = null,
        int? brandId = null, decimal? minPrice = null, decimal? maxPrice = null,
        string? sortBy = null, string? searchQuery = null, bool? discounted = null);
    Task<Product?> GetByIdAsync(int id);
    Task<Product?> GetBySlugAsync(string slug);
    Task<IEnumerable<Product>> GetFeaturedAsync(int count = 8);
    Task<IEnumerable<Product>> GetNewArrivalsAsync(int count = 8);
    Task<IEnumerable<Product>> GetDiscountedAsync(int count = 8);
    Task<IEnumerable<Product>> GetRelatedAsync(int productId, int categoryId, int count = 4);
    Task<Product> CreateAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(int id);
    Task<bool> SlugExistsAsync(string slug, int? excludeId = null);
    Task DecreaseStockAsync(int productId, int quantity);
}
