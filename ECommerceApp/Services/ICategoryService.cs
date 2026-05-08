using ECommerceApp.Models.Entities;

namespace ECommerceApp.Services;

public interface ICategoryService
{
    Task<IEnumerable<Category>> GetAllActiveAsync();
    Task<IEnumerable<Category>> GetMainCategoriesAsync();
    Task<Category?> GetByIdAsync(int id);
    Task<Category?> GetBySlugAsync(string slug);
    Task<Category> CreateAsync(Category category);
    Task UpdateAsync(Category category);
    Task DeleteAsync(int id);
    Task<bool> SlugExistsAsync(string slug, int? excludeId = null);
}
