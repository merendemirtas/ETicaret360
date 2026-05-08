# 🛒 .NET Core MVC E-Ticaret Projesi — Tam Geliştirme Promptu

## 🎯 Görev Tanımı

Sen deneyimli bir .NET Core backend geliştiricisisin. Aşağıda detayları verilen **orta ölçekli bir e-ticaret web uygulaması** geliştirmeni istiyorum. Proje **ASP.NET Core MVC** mimarisi ile, **tek proje** yapısında (Repository + Service pattern), **Entity Framework Core Code First** yaklaşımıyla yazılacak.

Kodu aşamalı olarak yaz. Her aşamayı tamamladıktan sonra bir sonrakine geç. Eksik bırakma, placeholder kullanma. Her dosyayı tam ve çalışır şekilde yaz.

---

## 🏗️ Teknoloji Stack

| Katman | Teknoloji |
|--------|-----------|
| Framework | ASP.NET Core MVC (.NET 8) |
| ORM | Entity Framework Core 8 (Code First) |
| Auth | ASP.NET Core Identity |
| Veritabanı | SQL Server (LocalDB geliştirme için) |
| Mapping | AutoMapper |
| Validasyon | FluentValidation |
| Görsel işleme | SixLabors.ImageSharp |
| URL | Slugify.Core |
| Mail | MailKit + MimeKit |
| Loglama | Serilog |
| Frontend | Bootstrap 5 + jQuery + Vanilla JS |

---

## 📁 Proje Klasör Yapısı

```
ECommerceApp/
├── Controllers/
│   ├── HomeController.cs
│   ├── ProductController.cs
│   ├── CartController.cs
│   ├── OrderController.cs
│   ├── AccountController.cs
│   ├── WishlistController.cs
│   ├── ReviewController.cs
│   └── Admin/
│       ├── DashboardController.cs
│       ├── ProductMgmtController.cs
│       ├── CategoryController.cs
│       ├── BrandController.cs
│       ├── OrderMgmtController.cs
│       └── CouponController.cs
├── Models/
│   ├── Entities/          ← EF Core entity sınıfları
│   ├── ViewModels/        ← View'a gönderilen modeller
│   └── DTOs/              ← Servisler arası veri transferi
├── Data/
│   ├── AppDbContext.cs
│   ├── SeedData.cs
│   └── Repositories/
│       ├── IRepository.cs
│       ├── Repository.cs
│       └── (entity bazlı repo'lar)
├── Services/
│   ├── IProductService.cs / ProductService.cs
│   ├── ICategoryService.cs / CategoryService.cs
│   ├── IOrderService.cs / OrderService.cs
│   ├── ICartService.cs / CartService.cs
│   ├── IWishlistService.cs / WishlistService.cs
│   ├── IReviewService.cs / ReviewService.cs
│   ├── ICouponService.cs / CouponService.cs
│   ├── IImageService.cs / ImageService.cs
│   └── IEmailService.cs / EmailService.cs
├── Helpers/
│   ├── SlugHelper.cs
│   ├── PaginationHelper.cs
│   └── FileHelper.cs
├── Mappings/
│   └── AutoMapperProfile.cs
├── Validators/
│   └── (FluentValidation sınıfları)
├── Views/
│   ├── Shared/
│   │   ├── _Layout.cshtml
│   │   ├── _AdminLayout.cshtml
│   │   ├── _Navbar.cshtml
│   │   ├── _Footer.cshtml
│   │   └── _Pagination.cshtml
│   ├── Home/
│   ├── Product/
│   ├── Cart/
│   ├── Order/
│   ├── Account/
│   ├── Wishlist/
│   └── Admin/
├── wwwroot/
│   ├── css/
│   ├── js/
│   └── uploads/
│       └── products/
├── appsettings.json
└── Program.cs
```

---

## 🗄️ Veritabanı — Entity Modelleri

Aşağıdaki tüm entity sınıflarını `Models/Entities/` altında oluştur.

### 1. ApplicationUser (Identity'den türetilmiş)
```csharp
public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    public ICollection<Order> Orders { get; set; }
    public ICollection<Address> Addresses { get; set; }
    public ICollection<CartItem> CartItems { get; set; }
    public ICollection<Review> Reviews { get; set; }
    public ICollection<Wishlist> Wishlists { get; set; }
}
```

### 2. Address
```csharp
public class Address
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public string Title { get; set; }         // "Ev", "İş"
    public string FullName { get; set; }
    public string Phone { get; set; }
    public string City { get; set; }
    public string District { get; set; }
    public string FullAddress { get; set; }
    public string PostalCode { get; set; }
    public bool IsDefault { get; set; } = false;
    public ApplicationUser User { get; set; }
}
```

### 3. Category
```csharp
public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public string? ImageUrl { get; set; }
    public int? ParentId { get; set; }        // Alt kategori desteği
    public Category? Parent { get; set; }
    public ICollection<Category> SubCategories { get; set; }
    public ICollection<Product> Products { get; set; }
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; } = 0;
}
```

### 4. Brand
```csharp
public class Brand
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? LogoUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<Product> Products { get; set; }
}
```

### 5. Product
```csharp
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public string Description { get; set; }
    public string? ShortDescription { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountedPrice { get; set; }
    public int Stock { get; set; }
    public int CategoryId { get; set; }
    public int BrandId { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsFeatured { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public Category Category { get; set; }
    public Brand Brand { get; set; }
    public ICollection<ProductImage> Images { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; }
    public ICollection<Review> Reviews { get; set; }
    public ICollection<CartItem> CartItems { get; set; }
    public ICollection<Wishlist> Wishlists { get; set; }
}
```

### 6. ProductImage
```csharp
public class ProductImage
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ImageUrl { get; set; }
    public bool IsMain { get; set; } = false;
    public int DisplayOrder { get; set; } = 0;
    public Product Product { get; set; }
}
```

### 7. CartItem
```csharp
public class CartItem
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    public ApplicationUser User { get; set; }
    public Product Product { get; set; }
}
```

### 8. Order
```csharp
public class Order
{
    public int Id { get; set; }
    public string OrderNumber { get; set; }   // "ORD-20240115-0001"
    public string UserId { get; set; }
    public int AddressId { get; set; }
    public int? CouponId { get; set; }
    public decimal SubTotal { get; set; }
    public decimal DiscountAmount { get; set; } = 0;
    public decimal ShippingCost { get; set; } = 0;
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public string? Note { get; set; }
    public DateTime OrderedAt { get; set; } = DateTime.UtcNow;
    public ApplicationUser User { get; set; }
    public Address Address { get; set; }
    public Coupon? Coupon { get; set; }
    public ICollection<OrderItem> Items { get; set; }
    public Payment? Payment { get; set; }
}

public enum OrderStatus
{
    Pending = 0,
    Confirmed = 1,
    Preparing = 2,
    Shipped = 3,
    Delivered = 4,
    Cancelled = 5,
    Refunded = 6
}
```

### 9. OrderItem
```csharp
public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; }   // Snapshot (ürün silinse bile kalsın)
    public string ProductImageUrl { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public Order Order { get; set; }
    public Product Product { get; set; }
}
```

### 10. Payment
```csharp
public class Payment
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string Provider { get; set; }      // "CreditCard", "BankTransfer", "COD"
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public decimal Amount { get; set; }
    public string? TransactionId { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Order Order { get; set; }
}

public enum PaymentStatus { Pending, Completed, Failed, Refunded }
```

### 11. Review
```csharp
public class Review
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public int ProductId { get; set; }
    public int Rating { get; set; }           // 1-5
    public string? Title { get; set; }
    public string Comment { get; set; }
    public bool IsApproved { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ApplicationUser User { get; set; }
    public Product Product { get; set; }
}
```

### 12. Wishlist
```csharp
public class Wishlist
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public int ProductId { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    public ApplicationUser User { get; set; }
    public Product Product { get; set; }
}
```

### 13. Coupon
```csharp
public class Coupon
{
    public int Id { get; set; }
    public string Code { get; set; }          // "YAZA20"
    public string? Description { get; set; }
    public DiscountType DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
    public decimal? MinOrderAmount { get; set; }
    public decimal? MaxDiscountAmount { get; set; }
    public int? UsageLimit { get; set; }
    public int UsedCount { get; set; } = 0;
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<Order> Orders { get; set; }
}

public enum DiscountType { Percentage, FixedAmount }
```

---

## 🔐 Authentication & Authorization

### Roller
- **Admin**: `[Authorize(Roles = "Admin")]` — tüm yönetim paneline erişim
- **Customer**: `[Authorize]` — sepet, sipariş, profil işlemleri

### Yapılacaklar
- `ApplicationUser`'ı Identity'den türet, ek alanlar ekle
- `Program.cs`'te Identity konfigüre et:
  - Şifre: minimum 8 karakter, büyük/küçük harf + rakam zorunlu
  - E-posta doğrulama zorunlu
  - Lockout: 5 başarısız denemede 15 dakika kilitle
- Seed data: `admin@site.com` / `Admin123!` kullanıcısı Admin rolüyle
- `AccountController` — Kayıt, Giriş, Çıkış, Profil, Şifre değiştirme

---

## 🛍️ Müşteri Tarafı Özellikler

### Ana Sayfa (HomeController)
- Öne çıkan ürünler (`IsFeatured = true`)
- Kategoriler (ana kategoriler, ikonlarıyla)
- Yeni eklenenler (son 8 ürün)
- İndirimli ürünler (DiscountedPrice olan ürünler)

### Ürün Listesi (ProductController / Index)
- Kategori bazlı filtreleme
- Marka bazlı filtreleme
- Fiyat aralığı filtresi
- Sıralama: Fiyat artan/azalan, Yeniden eskiye, En çok değerlendirilen
- Sayfalama: sayfa başına 12 ürün (PaginationHelper kullan)
- URL yapısı: `/urunler?kategori=elektronik&marka=apple&minFiyat=100&maxFiyat=500&sayfa=2`

### Ürün Detay (ProductController / Detail/{slug})
- Ürün görselleri galerisi (ana görsel + küçük resimler)
- Stok durumu göstergesi
- Sepete ekle butonu (AJAX)
- Favorilere ekle butonu (AJAX, giriş gerektir)
- Yorum formu (giriş gerektir, sadece satın almış kullanıcılar)
- Yorum listesi (onaylı yorumlar, rating yıldızları)
- İlgili ürünler (aynı kategoriden 4 ürün)

### Sepet (CartController)
- Sepeti görüntüle — ürün adı, görsel, fiyat, adet, ara toplam
- Adet güncelle (AJAX, +/- butonları)
- Ürünü kaldır (AJAX)
- Kupon uygula (AJAX) — geçerlilik kontrolü, indirim hesaplama
- Ara toplam, indirim, kargo, genel toplam özeti
- "Alışverişe Devam Et" ve "Siparişi Tamamla" butonları

### Checkout (OrderController / Checkout)
- Kayıtlı adresler listelenir, birini seç veya yeni ekle
- Ödeme yöntemi seçimi (Kredi Kartı simüle, Kapıda Ödeme)
- Sipariş özeti
- Sipariş oluşturulunca:
  1. Stok düşür
  2. Sepeti temizle
  3. Order kaydı oluştur
  4. Sipariş onay e-postası gönder (EmailService)
  5. `/siparis/tesekkur/{orderNumber}` sayfasına yönlendir

### Sipariş Takibi (OrderController / MyOrders)
- Kullanıcının tüm siparişleri
- Sipariş detay sayfası (kalemler, adres, durum takibi)

### Favori Listesi (WishlistController)
- Favori ürünler listesi
- Favoriden çıkar
- Favorideki ürünü sepete ekle

---

## ⚙️ Admin Paneli

Tüm admin controller'ları `[Area("Admin")]` veya `[Authorize(Roles = "Admin")]` ile korunacak. Ayrı `_AdminLayout.cshtml` kullanılacak.

### Dashboard (DashboardController)
- Toplam satış tutarı
- Toplam sipariş sayısı
- Toplam ürün sayısı
- Toplam kullanıcı sayısı
- Son 10 sipariş listesi
- Stok uyarısı (stock < 5 olan ürünler)

### Ürün Yönetimi (ProductMgmtController) — Tam CRUD
- **Listele**: DataTable veya sayfalı tablo, arama, filtreleme
- **Oluştur**:
  - Ad, açıklama, kısa açıklama
  - Kategori (dropdown)
  - Marka (dropdown)
  - Fiyat, indirimli fiyat
  - Stok
  - Slug (addan otomatik üretilir, düzenlenebilir)
  - Birden fazla görsel yükleme (ImageSharp ile resize: max 800x800)
  - Öne çıkar, Aktif checkbox
  - FluentValidation ile doğrulama
- **Düzenle**: Tüm alanlar, mevcut görseller (sil/ekle)
- **Sil**: Soft delete (IsActive = false)

### Kategori Yönetimi (CategoryController) — Tam CRUD
- Üst kategori seçimi ile hiyerarşik yapı
- Slug otomatik üretimi

### Marka Yönetimi (BrandController) — Tam CRUD
- Logo yükleme

### Sipariş Yönetimi (OrderMgmtController)
- Sipariş listesi (durum filtresi, tarih filtresi)
- Sipariş detay
- Durum güncelleme (dropdown: Onaylandı, Hazırlanıyor, Kargoya Verildi, Teslim Edildi, İptal)
- Kargo kodu girişi (Shipped durumunda)

### Yorum Yönetimi
- Onay bekleyen yorumlar listesi
- Onayla / Reddet

### Kupon Yönetimi (CouponController) — Tam CRUD
- Kupon kodu, tür (yüzde/tutar), değer
- Min sipariş tutarı, max indirim tutarı
- Kullanım limiti, son geçerlilik tarihi

---

## 🗂️ Repository & Service Katmanı

### Generic Repository
```csharp
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
}
```

Her servis için ayrı interface ve implementasyon yaz. Servisler business logic'i taşır, controller'lar sadece HTTP katmanını yönetir.

---

## 🎨 View Gereksinimleri

### Layout (_Layout.cshtml)
- Üst navbar: Logo, kategoriler dropdown, arama kutusu, sepet (ürün sayısı badge), kullanıcı menüsü
- Kategoriler navbar'da dinamik (veritabanından)
- Footer: Kategoriler, hızlı linkler, iletişim
- Bootstrap 5 kullan
- Toastr.js ile bildirimler

### Admin Layout (_AdminLayout.cshtml)
- Sol sidebar: navigasyon menüsü (Dashboard, Ürünler, Kategoriler, Markalar, Siparişler, Yorumlar, Kuponlar)
- Üst bar: breadcrumb, kullanıcı bilgisi, çıkış

### Responsive
- Tüm sayfalar mobil uyumlu olacak (Bootstrap grid)

---

## ⚙️ Program.cs Konfigürasyonu

```csharp
// Sırasıyla eklenecek servisler:
builder.Services.AddDbContext<AppDbContext>(...);
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(...);
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
// Tüm servisler AddScoped olarak eklenmeli
builder.Services.AddScoped<IProductService, ProductService>();
// ... diğer servisler

// Serilog konfigürasyonu
// FluentValidation konfigürasyonu
// Routing: slug destekli route
```

---

## 🌱 Seed Data

`SeedData.cs`'te aşağıdaki verileri oluştur:
- 2 rol: `Admin`, `Customer`
- 1 admin kullanıcı: `admin@site.com` / `Admin123!`
- 5 ana kategori (Elektronik, Giyim, Kitap, Spor, Ev & Yaşam)
- 3 marka
- 10 örnek ürün (kategori ve markalara bağlı, her birinde 2 görsel URL'si)
- 2 kupon: `HOSGELDIN10` (%10 indirim), `KARGO0` (sabit 0 TL kargo)

---

## 📧 E-posta Servisi

MailKit kullanarak:
- Sipariş onay maili (sipariş numarası, toplam tutar, kalemler)
- Şifre sıfırlama maili
- HTML template kullan (satır içi CSS ile)

Mail konfigürasyonu `appsettings.json`'da:
```json
"Email": {
  "Host": "smtp.gmail.com",
  "Port": 587,
  "Username": "",
  "Password": "",
  "FromName": "E-Ticaret"
}
```

---

## 🔍 Önemli Teknik Detaylar

### Görsel Yükleme
- Max boyut: 5MB
- Desteklenen formatlar: jpg, jpeg, png, webp
- ImageSharp ile otomatik resize: ürün görseli max 800x800, thumbnail max 200x200
- Kayıt yolu: `wwwroot/uploads/products/{productId}/`
- Veritabanına göreceli yol kaydet: `/uploads/products/{productId}/image.jpg`

### Slug Oluşturma
- Türkçe karakter desteği (ş→s, ı→i, ğ→g, ü→u, ö→o, ç→c)
- Boşluklar tire olur
- Benzersizlik kontrolü (aynı slug varsa -2, -3 ekle)

### Sayfalama
- `PaginatedList<T>` generic sınıfı
- View'da `_Pagination.cshtml` partial view

### AJAX İşlemleri (Sepet, Favori)
- Controller'dan JSON dön: `{ success: true, message: "...", data: {...} }`
- View'da jQuery ile yakala, Toastr ile bildirim göster

### Güvenlik
- CSRF token tüm formlarda
- XSS koruması (Razor otomatik encode eder)
- Kullanıcı sadece kendi siparişlerini/adreslerini görebilir (UserId kontrolü)
- Admin route'ları `[Authorize(Roles = "Admin")]` ile korunmalı

---

## 📋 Geliştirme Aşamaları (Sırayla Yaz)

### Aşama 1 — Altyapı
1. `appsettings.json` (connection string, mail config, app settings)
2. Tüm Entity sınıfları (`Models/Entities/`)
3. `AppDbContext.cs` (DbSet'ler, Fluent API konfigürasyonları, ilişkiler)
4. `Program.cs` (tüm servis kayıtları, middleware pipeline)
5. `SeedData.cs`
6. Migration oluştur ve uygula

### Aşama 2 — Repository & Servisler
7. `IRepository<T>` ve `Repository<T>`
8. Tüm servis interface ve implementasyonları
9. `AutoMapperProfile.cs` (entity ↔ ViewModel mappingleri)
10. `SlugHelper`, `PaginationHelper`, `FileHelper`

### Aşama 3 — Auth
11. `AccountController` (Register, Login, Logout, Profile, ChangePassword, ForgotPassword, ResetPassword)
12. Account View'ları (Register, Login, Profile)
13. E-posta doğrulama akışı

### Aşama 4 — Admin Paneli
14. `_AdminLayout.cshtml`
15. Dashboard
16. Ürün, Kategori, Marka CRUD
17. Sipariş yönetimi
18. Yorum onaylama
19. Kupon yönetimi

### Aşama 5 — Müşteri Tarafı
20. `_Layout.cshtml`, `_Navbar.cshtml`, `_Footer.cshtml`
21. Ana sayfa
22. Ürün listesi (filtreleme, sayfalama)
23. Ürün detay
24. Sepet (AJAX)
25. Checkout & sipariş oluşturma
26. Sipariş geçmişi
27. Favori listesi
28. Yorum ekleme

### Aşama 6 — Tamamlama
29. FluentValidation sınıfları
30. Hata sayfaları (404, 500)
31. `robots.txt`, `sitemap.xml` (basit)
32. Son test ve düzeltmeler

---

## ✅ Her Aşamada Uyulacak Kurallar

- Tüm async metotlarda `async/await` kullan, `Task<T>` dön
- `using` blokları yerine dependency injection ile `IDisposable` yönet
- Magic string yerine sabitler kullan (`Constants.cs`)
- Controller'da business logic olmasın, tüm mantık servislerde
- Her view'da model tipi `@model` ile belirtilmeli
- Türkçe kullanıcı mesajları (başarı/hata bildirimleri)
- `TempData` ile sayfa yönlendirme mesajları
- `ModelState.IsValid` kontrolü tüm POST action'larında
- Tüm resim URL'leri null-safe: `product.Images?.FirstOrDefault(x => x.IsMain)?.ImageUrl ?? "/images/no-image.png"`

---

## 🚀 Başlangıç Komutu

Şimdi **Aşama 1**'den başla. İlk olarak şunları yaz:
1. `appsettings.json`
2. Tüm entity sınıfları (13 adet)
3. `AppDbContext.cs`
4. `Program.cs`

Her dosyayı tam ve eksiksiz yaz. "// TODO" veya placeholder bırakma. Bir dosyayı bitirmeden diğerine geçme.
