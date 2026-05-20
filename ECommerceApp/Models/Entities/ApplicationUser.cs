using Microsoft.AspNetCore.Identity;

namespace ECommerceApp.Models.Entities;

// ASP.NET Identity'nin IdentityUser sınıfını genişleten uygulama kullanıcısı.
// Standart Identity alanlarına (Email, UserName, PasswordHash vb.) ek olarak
// projeye özel alanlar (FullName, CreatedAt, IsActive) ve ilişkili koleksiyonlar tutar.
public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;             // Ad Soyad
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;       // Kayıt tarihi (UTC)
    public bool IsActive { get; set; } = true;                        // Hesabın aktiflik durumu

    // İlişkili kayıtlar - EF Core navigation property'leri
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<Address> Addresses { get; set; } = new List<Address>();
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
