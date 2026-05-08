namespace ECommerceApp.Services;

public interface IEmailService
{
    Task SendOrderConfirmationAsync(string toEmail, string toName, string orderNumber, decimal totalAmount, string itemsSummary);
    Task SendPasswordResetAsync(string toEmail, string toName, string resetLink);
    Task SendEmailConfirmationAsync(string toEmail, string toName, string confirmLink);
}
