using ECommerceApp.Data.Repositories;
using ECommerceApp.Models.Entities;

namespace ECommerceApp.Services;

public class BrandService : IBrandService
{
    private readonly IRepository<Brand> _repo;

    public BrandService(IRepository<Brand> repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<Brand>> GetAllActiveAsync() =>
        await _repo.FindAsync(b => b.IsActive);

    public async Task<IEnumerable<Brand>> GetAllAsync() =>
        await _repo.GetAllAsync();

    public async Task<Brand?> GetByIdAsync(int id) =>
        await _repo.GetByIdAsync(id);

    public async Task<Brand> CreateAsync(Brand brand)
    {
        await _repo.AddAsync(brand);
        return brand;
    }

    public async Task UpdateAsync(Brand brand) =>
        await _repo.UpdateAsync(brand);

    public async Task DeleteAsync(int id)
    {
        var brand = await _repo.GetByIdAsync(id);
        if (brand is not null)
        {
            brand.IsActive = false;
            await _repo.UpdateAsync(brand);
        }
    }
}
