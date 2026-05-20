# ETicaret365

Tam kapsamlı, modern bir e-ticaret web uygulaması. ASP.NET Core 9 MVC mimarisiyle geliştirilmiş; ürün yönetimi, sipariş akışı, kupon sistemi, yorum moderasyonu ve admin paneli içerir.

---

## Teknoloji Yığını

| Katman | Teknoloji |
|---|---|
| Framework | ASP.NET Core 9 MVC |
| Veritabanı | PostgreSQL + Entity Framework Core 9 (Npgsql) |
| Kimlik Doğrulama | ASP.NET Core Identity |
| ORM Mapping | AutoMapper 13 |
| Validasyon | FluentValidation 11 |
| Görüntü İşleme | SixLabors ImageSharp 3 |
| E-posta | MailKit + MimeKit (SMTP) |
| Loglama | Serilog (Console + günlük dosya) |
| URL Slug | Slugify.Core |
| Frontend | Bootstrap 5, jQuery, Razor Views |

---

## Özellikler

### Kullanıcı Tarafı

- **Kayıt & Giriş** — E-posta onaylı kayıt, hesap kilitleme (5 hatalı giriş → 15 dk), şifre değiştirme
- **Ürün Listeleme** — Kategori, marka, fiyat aralığı ve metin filtresi; fiyat/puan/yenilik/öne çıkan sıralama; sayfalama
- **Ürün Detayı** — Çoklu görsel galeri, kısa & uzun açıklama, marka, stok durumu, indirimli fiyat, ilgili ürünler
- **SEO-Friendly URL** — Her ürün için otomatik benzersiz slug üretimi (`/urun/nike-air-max-2`)
- **Sepet** — Oturum açmadan da çalışan DB tabanlı sepet; ürün ekle/çıkar/miktar güncelle
- **Kupon Kodu** — Yüzde veya sabit indirim; minimum sipariş tutarı, maksimum indirim üst sınırı, kullanım limiti, son geçerlilik tarihi
- **Sipariş** — Adres seçimi, kargo hesabı (₺29,90 / ₺300 üzeri ücretsiz), sipariş özeti, sipariş numarası
- **Sipariş Takibi** — 7 aşamalı durum: Beklemede → Onaylandı → Hazırlanıyor → Kargoya Verildi → Teslim Edildi → İptal → İade
- **İstek Listesi** — Ürünleri favorilere ekle/çıkar (kullanıcı başına tekil kayıt)
- **Ürün Yorumları** — Puan + yorum gönder; onay bekleyen yorumlar admin onayına düşer

### Görüntü Yönetimi

- Ürün görselleri veritabanında `BYTEA` blob olarak saklanır (dosya sistemi bağımlılığı yok)
- Yükleme sırasında otomatik boyutlandırma (800×800 px max, JPEG çıktı)
- İlk yüklenen görsel otomatik olarak ana görsel seçilir
- Görsel URL'leri `/Image/Get/{id}` endpoint'i üzerinden sunulur

### Admin Paneli (`/admin/...`)

| Bölüm | Yapabilecekler |
|---|---|
| Dashboard | Toplam sipariş, gelir, ürün ve kullanıcı özeti |
| Ürün Yönetimi | Oluştur / Düzenle / Pasif yap; görsel yükle |
| Kategori Yönetimi | Alt-üst kategori hiyerarşisi |
| Marka Yönetimi | Marka ekle / düzenle |
| Sipariş Yönetimi | Sipariş listesi, durum güncelleme, detay görünümü |
| Kupon Yönetimi | Kupon oluştur / düzenle; yüzde & sabit tutar desteği |
| Yorum Moderasyonu | Onay bekleyen yorumları onayla veya sil |

---

## Mimari

```
ECommerceApp/
├── Controllers/
│   ├── Admin/          # Dashboard, ProductMgmt, Category, Brand, Coupon, OrderMgmt, ReviewMgmt
│   ├── AccountController.cs
│   ├── CartController.cs
│   ├── OrderController.cs
│   ├── ProductController.cs
│   ├── ReviewController.cs
│   ├── WishlistController.cs
│   └── ImageController.cs
├── Data/
│   ├── AppDbContext.cs          # IdentityDbContext türevi, 12 DbSet
│   ├── Repositories/            # Generic IRepository<T> pattern
│   └── SeedData.cs              # Uygulama başlangıcında admin + örnek veri
├── Models/
│   ├── Entities/                # EF Core entity'leri
│   └── ViewModels/              # Controller → View veri taşıyıcıları
├── Services/                    # Interface + Implementation çiftleri (10 servis)
├── Validators/                  # FluentValidation kuralları
├── Mappings/                    # AutoMapper profili
├── Helpers/                     # Slug, Pagination, File, Constants
├── Views/                       # Razor cshtml dosyaları
├── Logs/                        # Serilog günlük dosyaları
└── Program.cs                   # DI kaydı, middleware pipeline, seed
```

**Tasarım Kalıpları:**
- Repository Pattern (generic `IRepository<T>`)
- Service Layer (iş mantığı controller dışında)
- Dependency Injection (tüm servisler Scoped)
- ViewModel Pattern (entity'ler view'a doğrudan açılmaz)

---

## Kurulum

### Gereksinimler

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL 15+](https://www.postgresql.org/download/)

### Adımlar

**1. Repoyu klonla**
```bash
git clone https://github.com/kullanici-adi/ETicaret365.git
cd ETicaret365/ECommerceApp
```

**2. Veritabanı bağlantısını ayarla**

`appsettings.json` dosyasını düzenle:
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=ECommerceDb;Username=KULLANICI_ADI;Password=SIFRE"
}
```

**3. E-posta ayarını yapılandır** *(opsiyonel — kayıt onayı için gerekli)*
```json
"Email": {
  "Host": "smtp.gmail.com",
  "Port": 587,
  "Username": "ornek@gmail.com",
  "Password": "uygulama-sifresi"
}
```

**4. Migration & seed**

Uygulama ilk çalıştığında migration'ları otomatik uygular ve örnek verileri yükler.

```bash
dotnet run
```

> Uygulama açılışta `SeedData.InitializeAsync` ile admin hesabı ve örnek ürünler oluşturulur.

---

## Konfigürasyon

`appsettings.json` içindeki `AppSettings` bloğu:

| Parametre | Varsayılan | Açıklama |
|---|---|---|
| `ShippingCost` | 29.90 | Standart kargo ücreti (₺) |
| `FreeShippingThreshold` | 300.00 | Ücretsiz kargo eşiği (₺) |
| `MaxImageSizeMB` | 5 | Maksimum yükleme boyutu |
| `ProductImageMaxWidth` | 800 | Yeniden boyutlandırma genişliği (px) |
| `ProductImageMaxHeight` | 800 | Yeniden boyutlandırma yüksekliği (px) |
| `ThumbnailMaxWidth` | 200 | Küçük resim genişliği (px) |
| `ThumbnailMaxHeight` | 200 | Küçük resim yüksekliği (px) |

---


## Veritabanı Şeması (Özet)

```
ApplicationUser ──< Order ──< OrderItem >── Product
                 ──< CartItem >── Product
                 ──< Review >── Product
                 ──< Wishlist >── Product
                 ──< Address

Product >── Category (hiyerarşik, self-referencing)
Product >── Brand
Product ──< ProductImage (BYTEA blob)

Order >── Address
Order >── Coupon
Order ──  Payment (1-1)
```
<img width="755" height="583" alt="Ekran Resmi 2026-05-11 15 46 50" src="https://github.com/user-attachments/assets/761e1935-d953-45dc-a6dd-d95a9b8badfb" />

---

## Güvenlik

- Şifre politikası: min 8 karakter, büyük harf, küçük harf, rakam zorunlu
- E-posta onayı zorunlu (`RequireConfirmedEmail = true`)
- 5 hatalı girişte 15 dakika hesap kilitleme
- Cookie: `HttpOnly`, 7 gün sliding expiration
- HTTPS yönlendirme + HSTS (production)
- Session: `HttpOnly` + `IsEssential` cookie
- Admin rotaları için role-based authorization

---

## Loglama

Serilog ile yapılandırılmış; iki sink aktif:

- **Console** — geliştirme ortamında anlık takip
- **File** — `Logs/log-YYYYMMDD.txt` formatında günlük dönen dosyalar

`Microsoft.*` ve `System.*` namespace'leri `Warning` seviyesinde filtrelenir; uygulama logları `Information` ve üzeri kaydedilir.

---


## Ekran Resimleri 

<img width="1709" height="990" alt="Ekran Resmi 2026-05-20 16 31 12" src="https://github.com/user-attachments/assets/d5ef33b1-1204-4918-b595-97ca7ce98e3a" />

<img width="1710" height="1112" alt="Ekran Resmi 2026-05-09 22 39 50" src="https://github.com/user-attachments/assets/aa97eebb-5bfb-4a7c-857d-9dc3d71e76ba" />

<img width="1710" height="1112" alt="Ekran Resmi 2026-05-09 22 42 40" src="https://github.com/user-attachments/assets/fd33e9c8-1890-4e5d-9a96-cefa16b79e65" />

<img width="1710" height="1112" alt="Ekran Resmi 2026-05-09 22 43 13" src="https://github.com/user-attachments/assets/dbbb5774-8354-4a3b-bf4d-6a663527ab8e" />

<img width="1709" height="990" alt="Ekran Resmi 2026-05-20 16 40 14" src="https://github.com/user-attachments/assets/c583487b-4cd8-4c6c-a15b-be58f68353a2" />

<img width="1710" height="1112" alt="Ekran Resmi 2026-05-09 22 43 39" src="https://github.com/user-attachments/assets/abbfcc10-a701-44a3-b563-fc99139c17c2" />

