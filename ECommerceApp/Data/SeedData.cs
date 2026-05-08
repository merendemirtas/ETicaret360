using ECommerceApp.Helpers;
using ECommerceApp.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace ECommerceApp.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var context = serviceProvider.GetRequiredService<AppDbContext>();

        await SeedRolesAsync(roleManager);
        await SeedAdminUserAsync(userManager);
        await SeedCategoriesAsync(context);
        await SeedBrandsAsync(context);
        await SeedProductsAsync(context);
        await SeedCouponsAsync(context);
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        string[] roles = [Constants.AdminRole, Constants.CustomerRole];
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    private static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager)
    {
        if (await userManager.FindByEmailAsync(Constants.AdminEmail) is not null) return;

        var admin = new ApplicationUser
        {
            UserName = Constants.AdminEmail,
            Email = Constants.AdminEmail,
            FullName = "Site Yöneticisi",
            EmailConfirmed = true,
            IsActive = true
        };

        var result = await userManager.CreateAsync(admin, Constants.AdminPassword);
        if (result.Succeeded)
            await userManager.AddToRoleAsync(admin, Constants.AdminRole);
    }

    private static async Task SeedCategoriesAsync(AppDbContext context)
    {
        if (context.Categories.Any()) return;

        var categories = new List<Category>
        {
            new() { Name = "Elektronik", Slug = "elektronik", IsActive = true, DisplayOrder = 1 },
            new() { Name = "Giyim", Slug = "giyim", IsActive = true, DisplayOrder = 2 },
            new() { Name = "Kitap", Slug = "kitap", IsActive = true, DisplayOrder = 3 },
            new() { Name = "Spor", Slug = "spor", IsActive = true, DisplayOrder = 4 },
            new() { Name = "Ev & Yaşam", Slug = "ev-yasam", IsActive = true, DisplayOrder = 5 }
        };

        context.Categories.AddRange(categories);
        await context.SaveChangesAsync();
    }

    private static async Task SeedBrandsAsync(AppDbContext context)
    {
        if (context.Brands.Any()) return;

        var brands = new List<Brand>
        {
            new() { Name = "TechPro", IsActive = true },
            new() { Name = "ModaStyle", IsActive = true },
            new() { Name = "SportMax", IsActive = true }
        };

        context.Brands.AddRange(brands);
        await context.SaveChangesAsync();
    }

    private static async Task SeedProductsAsync(AppDbContext context)
    {
        if (context.Products.Any()) return;

        var elektronik = context.Categories.First(c => c.Slug == "elektronik");
        var giyim = context.Categories.First(c => c.Slug == "giyim");
        var kitap = context.Categories.First(c => c.Slug == "kitap");
        var spor = context.Categories.First(c => c.Slug == "spor");
        var evYasam = context.Categories.First(c => c.Slug == "ev-yasam");

        var techPro = context.Brands.First(b => b.Name == "TechPro");
        var modaStyle = context.Brands.First(b => b.Name == "ModaStyle");
        var sportMax = context.Brands.First(b => b.Name == "SportMax");

        var products = new List<Product>
        {
            new()
            {
                Name = "Akıllı Telefon X500",
                Slug = "akilli-telefon-x500",
                Description = "Yüksek performanslı akıllı telefon, 6.7 inç ekran, 256GB depolama.",
                ShortDescription = "6.7\" ekran, 256GB, 5G destekli",
                Price = 12999,
                DiscountedPrice = 10999,
                Stock = 50,
                CategoryId = elektronik.Id,
                BrandId = techPro.Id,
                IsActive = true,
                IsFeatured = true,
                Images = new List<ProductImage>
                {
                    new() { ImageUrl = "/images/no-image.png", IsMain = true, DisplayOrder = 1 },
                    new() { ImageUrl = "/images/no-image.png", IsMain = false, DisplayOrder = 2 }
                }
            },
            new()
            {
                Name = "Kablosuz Kulaklık Pro",
                Slug = "kablosuz-kulaklik-pro",
                Description = "Aktif gürültü engelleyici, 30 saat pil ömrü.",
                ShortDescription = "ANC, 30 saat pil, Bluetooth 5.3",
                Price = 2499,
                Stock = 100,
                CategoryId = elektronik.Id,
                BrandId = techPro.Id,
                IsActive = true,
                IsFeatured = true,
                Images = new List<ProductImage>
                {
                    new() { ImageUrl = "/images/no-image.png", IsMain = true, DisplayOrder = 1 },
                    new() { ImageUrl = "/images/no-image.png", IsMain = false, DisplayOrder = 2 }
                }
            },
            new()
            {
                Name = "Laptop UltraBook",
                Slug = "laptop-ultrabook",
                Description = "14 inç, Intel Core i7, 16GB RAM, 512GB SSD.",
                ShortDescription = "14\", i7, 16GB RAM, 512GB SSD",
                Price = 24999,
                DiscountedPrice = 21999,
                Stock = 30,
                CategoryId = elektronik.Id,
                BrandId = techPro.Id,
                IsActive = true,
                IsFeatured = true,
                Images = new List<ProductImage>
                {
                    new() { ImageUrl = "/images/no-image.png", IsMain = true, DisplayOrder = 1 },
                    new() { ImageUrl = "/images/no-image.png", IsMain = false, DisplayOrder = 2 }
                }
            },
            new()
            {
                Name = "Erkek Slim Fit Gömlek",
                Slug = "erkek-slim-fit-gomlek",
                Description = "Yüksek kaliteli pamuklu slim fit gömlek.",
                ShortDescription = "Pamuklu, slim fit, çeşitli renk seçenekleri",
                Price = 299,
                Stock = 200,
                CategoryId = giyim.Id,
                BrandId = modaStyle.Id,
                IsActive = true,
                IsFeatured = false,
                Images = new List<ProductImage>
                {
                    new() { ImageUrl = "/images/no-image.png", IsMain = true, DisplayOrder = 1 },
                    new() { ImageUrl = "/images/no-image.png", IsMain = false, DisplayOrder = 2 }
                }
            },
            new()
            {
                Name = "Kadın Trençkot",
                Slug = "kadin-trencot",
                Description = "Şık ve modern trençkot, bej renk.",
                ShortDescription = "Klasik kesim, bej, tam boy",
                Price = 1299,
                DiscountedPrice = 999,
                Stock = 75,
                CategoryId = giyim.Id,
                BrandId = modaStyle.Id,
                IsActive = true,
                IsFeatured = true,
                Images = new List<ProductImage>
                {
                    new() { ImageUrl = "/images/no-image.png", IsMain = true, DisplayOrder = 1 },
                    new() { ImageUrl = "/images/no-image.png", IsMain = false, DisplayOrder = 2 }
                }
            },
            new()
            {
                Name = "Yazılım Mühendisliği",
                Slug = "yazilim-muhendisligi-kitap",
                Description = "Modern yazılım mühendisliği prensipleri ve uygulamaları.",
                ShortDescription = "560 sayfa, kapsamlı referans kitabı",
                Price = 189,
                Stock = 150,
                CategoryId = kitap.Id,
                BrandId = techPro.Id,
                IsActive = true,
                IsFeatured = false,
                Images = new List<ProductImage>
                {
                    new() { ImageUrl = "/images/no-image.png", IsMain = true, DisplayOrder = 1 },
                    new() { ImageUrl = "/images/no-image.png", IsMain = false, DisplayOrder = 2 }
                }
            },
            new()
            {
                Name = "Yoga Matı Premium",
                Slug = "yoga-mati-premium",
                Description = "6mm kalınlığında kaymaz yoga matı.",
                ShortDescription = "6mm, kaymaz, çevre dostu malzeme",
                Price = 449,
                Stock = 120,
                CategoryId = spor.Id,
                BrandId = sportMax.Id,
                IsActive = true,
                IsFeatured = false,
                Images = new List<ProductImage>
                {
                    new() { ImageUrl = "/images/no-image.png", IsMain = true, DisplayOrder = 1 },
                    new() { ImageUrl = "/images/no-image.png", IsMain = false, DisplayOrder = 2 }
                }
            },
            new()
            {
                Name = "Koşu Ayakkabısı Speed",
                Slug = "kosu-ayakkabisi-speed",
                Description = "Hafif ve hava geçirgen koşu ayakkabısı.",
                ShortDescription = "Hafif, hava geçirgen, her zemin",
                Price = 899,
                DiscountedPrice = 749,
                Stock = 80,
                CategoryId = spor.Id,
                BrandId = sportMax.Id,
                IsActive = true,
                IsFeatured = true,
                Images = new List<ProductImage>
                {
                    new() { ImageUrl = "/images/no-image.png", IsMain = true, DisplayOrder = 1 },
                    new() { ImageUrl = "/images/no-image.png", IsMain = false, DisplayOrder = 2 }
                }
            },
            new()
            {
                Name = "Kahve Makinesi Deluxe",
                Slug = "kahve-makinesi-deluxe",
                Description = "Espresso, cappuccino ve latte yapabilen otomatik kahve makinesi.",
                ShortDescription = "15 bar basınç, milk frother dahil",
                Price = 3499,
                Stock = 40,
                CategoryId = evYasam.Id,
                BrandId = techPro.Id,
                IsActive = true,
                IsFeatured = true,
                Images = new List<ProductImage>
                {
                    new() { ImageUrl = "/images/no-image.png", IsMain = true, DisplayOrder = 1 },
                    new() { ImageUrl = "/images/no-image.png", IsMain = false, DisplayOrder = 2 }
                }
            },
            new()
            {
                Name = "Dekoratif Yastık Seti",
                Slug = "dekoratif-yastik-seti",
                Description = "4'lü dekoratif yastık seti, %100 pamuk.",
                ShortDescription = "4'lü set, pamuklu, çeşitli desenler",
                Price = 259,
                Stock = 3,
                CategoryId = evYasam.Id,
                BrandId = modaStyle.Id,
                IsActive = true,
                IsFeatured = false,
                Images = new List<ProductImage>
                {
                    new() { ImageUrl = "/images/no-image.png", IsMain = true, DisplayOrder = 1 },
                    new() { ImageUrl = "/images/no-image.png", IsMain = false, DisplayOrder = 2 }
                }
            }
        };

        context.Products.AddRange(products);
        await context.SaveChangesAsync();
    }

    private static async Task SeedCouponsAsync(AppDbContext context)
    {
        if (context.Coupons.Any()) return;

        var coupons = new List<Coupon>
        {
            new()
            {
                Code = "HOSGELDIN10",
                Description = "Hoş geldin indirimi %10",
                DiscountType = DiscountType.Percentage,
                DiscountValue = 10,
                MinOrderAmount = 100,
                MaxDiscountAmount = 200,
                UsageLimit = 1000,
                IsActive = true,
                ExpiresAt = DateTime.UtcNow.AddYears(1)
            },
            new()
            {
                Code = "KARGO0",
                Description = "Ücretsiz kargo kuponu",
                DiscountType = DiscountType.FixedAmount,
                DiscountValue = 29.90m,
                MinOrderAmount = 150,
                IsActive = true,
                ExpiresAt = DateTime.UtcNow.AddYears(1)
            }
        };

        context.Coupons.AddRange(coupons);
        await context.SaveChangesAsync();
    }
}
