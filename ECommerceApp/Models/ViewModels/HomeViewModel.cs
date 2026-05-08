using ECommerceApp.Models.Entities;

namespace ECommerceApp.Models.ViewModels;

public class HomeViewModel
{
    public IEnumerable<Product> FeaturedProducts { get; set; } = new List<Product>();
    public IEnumerable<Product> NewArrivals { get; set; } = new List<Product>();
    public IEnumerable<Product> DiscountedProducts { get; set; } = new List<Product>();
    public IEnumerable<Category> MainCategories { get; set; } = new List<Category>();
}
