using ECommerceApp.Data.Repositories;
using ECommerceApp.Models.Entities;

namespace ECommerceApp.Services;

public class CouponService : ICouponService
{
    private readonly IRepository<Coupon> _repo;

    public CouponService(IRepository<Coupon> repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<Coupon>> GetAllAsync() =>
        await _repo.GetAllAsync();

    public async Task<Coupon?> GetByIdAsync(int id) =>
        await _repo.GetByIdAsync(id);

    public async Task<Coupon?> GetByCodeAsync(string code) =>
        (await _repo.FindAsync(c => c.Code.ToUpper() == code.ToUpper())).FirstOrDefault();

    public async Task<(bool isValid, string message, decimal discount)> ValidateCouponAsync(string code, decimal orderAmount)
    {
        var coupon = await GetByCodeAsync(code);

        if (coupon is null)
            return (false, "Geçersiz kupon kodu.", 0);

        if (!coupon.IsActive)
            return (false, "Bu kupon aktif değil.", 0);

        if (coupon.ExpiresAt.HasValue && coupon.ExpiresAt < DateTime.UtcNow)
            return (false, "Bu kuponun süresi dolmuş.", 0);

        if (coupon.UsageLimit.HasValue && coupon.UsedCount >= coupon.UsageLimit)
            return (false, "Bu kupon kullanım limitine ulaşmış.", 0);

        if (coupon.MinOrderAmount.HasValue && orderAmount < coupon.MinOrderAmount)
            return (false, $"Bu kupon için minimum sipariş tutarı {coupon.MinOrderAmount:C2} olmalıdır.", 0);

        var discount = coupon.DiscountType == DiscountType.Percentage
            ? orderAmount * coupon.DiscountValue / 100
            : coupon.DiscountValue;

        if (coupon.MaxDiscountAmount.HasValue)
            discount = Math.Min(discount, coupon.MaxDiscountAmount.Value);

        return (true, $"Kupon uygulandı! {discount:C2} indirim kazandınız.", discount);
    }

    public async Task<Coupon> CreateAsync(Coupon coupon)
    {
        coupon.Code = coupon.Code.ToUpper();
        await _repo.AddAsync(coupon);
        return coupon;
    }

    public async Task UpdateAsync(Coupon coupon)
    {
        coupon.Code = coupon.Code.ToUpper();
        await _repo.UpdateAsync(coupon);
    }

    public async Task DeleteAsync(int id)
    {
        var coupon = await _repo.GetByIdAsync(id);
        if (coupon is not null)
            await _repo.DeleteAsync(coupon);
    }
}
