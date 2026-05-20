namespace ECommerceApp.Models.Entities;

// Kullanıcının teslimat / fatura adresi. Bir kullanıcının birden fazla adresi olabilir
// ve sadece biri varsayılan (IsDefault) olarak işaretlenir.
public class Address
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;     // FK -> ApplicationUser.Id
    public string Title { get; set; } = string.Empty;      // Adres etiketi ("Ev", "İş" vb.)
    public string FullName { get; set; } = string.Empty;   // Teslim alacak kişinin adı soyadı
    public string Phone { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string FullAddress { get; set; } = string.Empty; // Açık adres satırı
    public string PostalCode { get; set; } = string.Empty;
    public bool IsDefault { get; set; } = false;            // Sipariş ekranında ön-seçili gelen adres

    // Navigation property - EF Core ilişki kurma için kullanılır
    public ApplicationUser User { get; set; } = null!;
}
