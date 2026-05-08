using AutoMapper;
using ECommerceApp.Models.Entities;
using ECommerceApp.Models.ViewModels;

namespace ECommerceApp.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Product, ProductCardViewModel>()
            .ForMember(d => d.MainImageUrl, o => o.MapFrom(s =>
                s.Images != null && s.Images.Any(i => i.IsMain)
                    ? s.Images.First(i => i.IsMain).ImageUrl
                    : "/images/no-image.png"))
            .ForMember(d => d.BrandName, o => o.MapFrom(s => s.Brand != null ? s.Brand.Name : ""))
            .ForMember(d => d.AverageRating, o => o.MapFrom(s =>
                s.Reviews != null && s.Reviews.Any() ? s.Reviews.Average(r => r.Rating) : 0))
            .ForMember(d => d.ReviewCount, o => o.MapFrom(s =>
                s.Reviews != null ? s.Reviews.Count : 0));

        CreateMap<Product, ProductCreateEditViewModel>()
            .ForMember(d => d.ExistingImages, o => o.MapFrom(s => s.Images.ToList()))
            .ForMember(d => d.Images, o => o.Ignore());

        CreateMap<ProductCreateEditViewModel, Product>()
            .ForMember(d => d.Images, o => o.Ignore())
            .ForMember(d => d.Category, o => o.Ignore())
            .ForMember(d => d.Brand, o => o.Ignore());

        CreateMap<AddressFormViewModel, Address>();
        CreateMap<Address, AddressFormViewModel>();

        CreateMap<ProfileViewModel, ApplicationUser>()
            .ForMember(d => d.UserName, o => o.Ignore())
            .ForMember(d => d.Email, o => o.Ignore());

        CreateMap<ApplicationUser, ProfileViewModel>()
            .ForMember(d => d.Email, o => o.MapFrom(s => s.Email));
    }
}
