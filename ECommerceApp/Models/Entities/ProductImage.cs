namespace ECommerceApp.Models.Entities;

public class ProductImage
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsMain { get; set; } = false;
    public int DisplayOrder { get; set; } = 0;
    public byte[]? ImageData { get; set; }
    public string ContentType { get; set; } = "image/jpeg";
    public Product Product { get; set; } = null!;
}
