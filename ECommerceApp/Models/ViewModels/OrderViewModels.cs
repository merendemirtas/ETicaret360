using ECommerceApp.Models.Entities;

namespace ECommerceApp.Models.ViewModels;

public class CheckoutViewModel
{
    public List<Address> Addresses { get; set; } = new();
    public int? SelectedAddressId { get; set; }
    public string PaymentProvider { get; set; } = "CreditCard";
    public string? Note { get; set; }
    public decimal SubTotal { get; set; }
    public decimal Discount { get; set; }
    public decimal Shipping { get; set; }
    public decimal Total { get; set; }
    public int? CouponId { get; set; }
    public string? CouponCode { get; set; }
    public AddressFormViewModel NewAddress { get; set; } = new();
}

public class AddressFormViewModel
{
    public string Title { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string FullAddress { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
}

public class OrderDetailViewModel
{
    public Order Order { get; set; } = null!;
    public string StatusText => Order.Status switch
    {
        OrderStatus.Pending => "Beklemede",
        OrderStatus.Confirmed => "Onaylandı",
        OrderStatus.Preparing => "Hazırlanıyor",
        OrderStatus.Shipped => "Kargoya Verildi",
        OrderStatus.Delivered => "Teslim Edildi",
        OrderStatus.Cancelled => "İptal Edildi",
        OrderStatus.Refunded => "İade Edildi",
        _ => "Bilinmiyor"
    };
    public string StatusClass => Order.Status switch
    {
        OrderStatus.Pending => "warning",
        OrderStatus.Confirmed => "info",
        OrderStatus.Preparing => "primary",
        OrderStatus.Shipped => "info",
        OrderStatus.Delivered => "success",
        OrderStatus.Cancelled => "danger",
        OrderStatus.Refunded => "secondary",
        _ => "secondary"
    };
}
