namespace ECommerceApp.Models.Entities;

// Ürün markası. Bir markanın birden çok ürünü olabilir (1-N ilişki).
public class Brand
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;   // Marka adı (Nike, Apple vb.)
    public string? LogoUrl { get; set; }               // Marka logosu (opsiyonel, wwwroot altındaki göreli yol)
    public bool IsActive { get; set; } = true;         // Pasif markalar listeden gizlenir
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
