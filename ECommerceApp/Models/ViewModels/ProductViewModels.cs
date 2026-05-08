using ECommerceApp.Models.Entities;

namespace ECommerceApp.Models.ViewModels;

public class ProductListViewModel
{
    public IEnumerable<ProductCardViewModel> Products { get; set; } = new List<ProductCardViewModel>();
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public int? CategoryId { get; set; }
    public int? BrandId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? SortBy { get; set; }
    public string? SearchQuery { get; set; }
    public bool? Indirimli { get; set; }
    public IEnumerable<Category> Categories { get; set; } = new List<Category>();
    public IEnumerable<Brand> Brands { get; set; } = new List<Brand>();
}

public class ProductCardViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountedPrice { get; set; }
    public string MainImageUrl { get; set; } = "/images/no-image.png";
    public string BrandName { get; set; } = string.Empty;
    public bool IsInWishlist { get; set; }
    public int Stock { get; set; }
    public double AverageRating { get; set; }
    public int ReviewCount { get; set; }
}

public class ProductDetailViewModel
{
    public Product Product { get; set; } = null!;
    public bool IsInWishlist { get; set; }
    public bool CanReview { get; set; }
    public bool HasReviewed { get; set; }
    public IEnumerable<Product> RelatedProducts { get; set; } = new List<Product>();
    public double AverageRating { get; set; }
}

public class ProductCreateEditViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountedPrice { get; set; }
    public int Stock { get; set; }
    public int CategoryId { get; set; }
    public int BrandId { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsFeatured { get; set; } = false;
    public string? Slug { get; set; }
    public List<IFormFile> Images { get; set; } = new();
    public List<ProductImage> ExistingImages { get; set; } = new();
    public IEnumerable<Category> Categories { get; set; } = new List<Category>();
    public IEnumerable<Brand> Brands { get; set; } = new List<Brand>();
}
