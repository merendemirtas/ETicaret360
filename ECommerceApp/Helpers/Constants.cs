namespace ECommerceApp.Helpers;

// Uygulama genelinde kullanılan sabit değerler (rol adları, varsayılan admin bilgileri, sayfa boyutları vb.)
public static class Constants
{
    // Identity rolleri - SeedData ve [Authorize(Roles = ...)] içinde kullanılır
    public const string AdminRole = "Admin";
    public const string CustomerRole = "Customer";

    // İlk açılışta otomatik oluşturulan varsayılan admin hesabı bilgileri
    public const string AdminEmail = "admin@site.com";
    public const string AdminPassword = "Admin123!";

    // Görseli olmayan ürünler için kullanılan placeholder yolu
    public const string NoImagePath = "/images/no-image.png";

    // Sayfalama varsayılanları: müşteri tarafı 12, admin paneli 20 kayıt/sayfa
    public const int PageSize = 12;
    public const int AdminPageSize = 20;
}
