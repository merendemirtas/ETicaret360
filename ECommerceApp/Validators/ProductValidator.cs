using ECommerceApp.Models.ViewModels;
using FluentValidation;

namespace ECommerceApp.Validators;

public class ProductValidator : AbstractValidator<ProductCreateEditViewModel>
{
    public ProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Ürün adı zorunludur.")
            .MaximumLength(200).WithMessage("Ürün adı en fazla 200 karakter olabilir.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Açıklama zorunludur.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Fiyat 0'dan büyük olmalıdır.");

        RuleFor(x => x.DiscountedPrice)
            .LessThan(x => x.Price).When(x => x.DiscountedPrice.HasValue)
            .WithMessage("İndirimli fiyat normal fiyattan küçük olmalıdır.");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("Stok 0 veya daha büyük olmalıdır.");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("Kategori seçiniz.");

        RuleFor(x => x.BrandId)
            .GreaterThan(0).WithMessage("Marka seçiniz.");
    }
}

public class CouponValidator : AbstractValidator<ECommerceApp.Models.Entities.Coupon>
{
    public CouponValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Kupon kodu zorunludur.")
            .MaximumLength(50).WithMessage("Kupon kodu en fazla 50 karakter olabilir.");

        RuleFor(x => x.DiscountValue)
            .GreaterThan(0).WithMessage("İndirim değeri 0'dan büyük olmalıdır.");

        RuleFor(x => x.DiscountValue)
            .LessThanOrEqualTo(100)
            .When(x => x.DiscountType == ECommerceApp.Models.Entities.DiscountType.Percentage)
            .WithMessage("Yüzde indirim 100'den fazla olamaz.");
    }
}
