using ECommerceApp.Data.Repositories;
using ECommerceApp.Helpers;
using ECommerceApp.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Services;

public class CategoryService : ICategoryService
{
    private readonly IRepository<Category> _repo;

    public CategoryService(IRepository<Category> repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<Category>> GetAllActiveAsync() =>
        await _repo.FindAsync(c => c.IsActive);

    public async Task<IEnumerable<Category>> GetMainCategoriesAsync() =>
        await _repo.FindAsync(c => c.IsActive && c.ParentId == null);

    public async Task<Category?> GetByIdAsync(int id) =>
        await _repo.Query()
            .Include(c => c.SubCategories)
            .Include(c => c.Parent)
            .FirstOrDefaultAsync(c => c.Id == id);

    public async Task<Category?> GetBySlugAsync(string slug) =>
        await _repo.Query()
            .Include(c => c.SubCategories)
            .FirstOrDefaultAsync(c => c.Slug == slug && c.IsActive);

    public async Task<Category> CreateAsync(Category category)
    {
        category.Slug = await GenerateUniqueSlugAsync(SlugHelper.GenerateSlug(category.Name));
        await _repo.AddAsync(category);
        return category;
    }

    public async Task UpdateAsync(Category category)
    {
        await _repo.UpdateAsync(category);
    }

    public async Task DeleteAsync(int id)
    {
        var category = await _repo.GetByIdAsync(id);
        if (category is not null)
        {
            category.IsActive = false;
            await _repo.UpdateAsync(category);
        }
    }

    public async Task<bool> SlugExistsAsync(string slug, int? excludeId = null) =>
        await _repo.ExistsAsync(c => c.Slug == slug && (!excludeId.HasValue || c.Id != excludeId.Value));

    private async Task<string> GenerateUniqueSlugAsync(string baseSlug)
    {
        var existing = (await _repo.GetAllAsync()).Select(c => c.Slug).ToList();
        return SlugHelper.MakeUnique(baseSlug, existing);
    }
}
