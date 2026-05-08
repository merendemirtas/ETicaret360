using ECommerceApp.Models.Entities;

namespace ECommerceApp.Services;

public interface ICouponService
{
    Task<IEnumerable<Coupon>> GetAllAsync();
    Task<Coupon?> GetByIdAsync(int id);
    Task<Coupon?> GetByCodeAsync(string code);
    Task<(bool isValid, string message, decimal discount)> ValidateCouponAsync(string code, decimal orderAmount);
    Task<Coupon> CreateAsync(Coupon coupon);
    Task UpdateAsync(Coupon coupon);
    Task DeleteAsync(int id);
}
