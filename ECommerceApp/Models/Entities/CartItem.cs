namespace ECommerceApp.Models.Entities;

// Kullanıcının sepetindeki bir ürün satırı. (UserId, ProductId) çifti benzersizdir;
// aynı ürün tekrar eklendiğinde yeni satır yerine Quantity artırılır (CartService).
public class CartItem
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;   // FK -> ApplicationUser
    public int ProductId { get; set; }                   // FK -> Product
    public int Quantity { get; set; }                    // Sepetteki adet
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;

    public ApplicationUser User { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
