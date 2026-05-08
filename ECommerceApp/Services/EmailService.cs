using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace ECommerceApp.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    private async Task SendAsync(string toEmail, string toName, string subject, string htmlBody)
    {
        var host = _config["Email:Host"] ?? "smtp.gmail.com";
        var port = int.Parse(_config["Email:Port"] ?? "587");
        var username = _config["Email:Username"] ?? "";
        var password = _config["Email:Password"] ?? "";
        var fromName = _config["Email:FromName"] ?? "E-Ticaret";

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) return;

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(fromName, username));
        message.To.Add(new MailboxAddress(toName, toEmail));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = htmlBody };

        using var client = new SmtpClient();
        await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(username, password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }

    public async Task SendOrderConfirmationAsync(string toEmail, string toName, string orderNumber, decimal totalAmount, string itemsSummary)
    {
        var html = $"""
            <html><body style="font-family:Arial,sans-serif;max-width:600px;margin:auto;">
            <div style="background:#2c3e50;padding:20px;text-align:center;">
              <h1 style="color:white;margin:0;">Siparişiniz Alındı!</h1>
            </div>
            <div style="padding:30px;background:#f9f9f9;">
              <p>Merhaba <strong>{toName}</strong>,</p>
              <p>Siparişiniz başarıyla oluşturuldu.</p>
              <div style="background:white;padding:20px;border-radius:8px;margin:20px 0;">
                <p><strong>Sipariş No:</strong> {orderNumber}</p>
                <p><strong>Toplam Tutar:</strong> {totalAmount:C2}</p>
                <hr/>
                <p><strong>Ürünler:</strong></p>
                {itemsSummary}
              </div>
              <p>Siparişinizi hesabınızdan takip edebilirsiniz.</p>
            </div>
            </body></html>
            """;

        await SendAsync(toEmail, toName, $"Sipariş Onayı - {orderNumber}", html);
    }

    public async Task SendPasswordResetAsync(string toEmail, string toName, string resetLink)
    {
        var html = $"""
            <html><body style="font-family:Arial,sans-serif;max-width:600px;margin:auto;">
            <div style="background:#2c3e50;padding:20px;text-align:center;">
              <h1 style="color:white;margin:0;">Şifre Sıfırlama</h1>
            </div>
            <div style="padding:30px;background:#f9f9f9;">
              <p>Merhaba <strong>{toName}</strong>,</p>
              <p>Şifrenizi sıfırlamak için aşağıdaki butona tıklayın:</p>
              <div style="text-align:center;margin:30px 0;">
                <a href="{resetLink}" style="background:#e74c3c;color:white;padding:14px 28px;text-decoration:none;border-radius:6px;font-weight:bold;">
                  Şifremi Sıfırla
                </a>
              </div>
              <p style="color:#999;font-size:12px;">Bu link 24 saat geçerlidir. Eğer şifre sıfırlama talebinde bulunmadıysanız bu e-postayı dikkate almayın.</p>
            </div>
            </body></html>
            """;

        await SendAsync(toEmail, toName, "Şifre Sıfırlama", html);
    }

    public async Task SendEmailConfirmationAsync(string toEmail, string toName, string confirmLink)
    {
        var html = $"""
            <html><body style="font-family:Arial,sans-serif;max-width:600px;margin:auto;">
            <div style="background:#2c3e50;padding:20px;text-align:center;">
              <h1 style="color:white;margin:0;">E-posta Doğrulama</h1>
            </div>
            <div style="padding:30px;background:#f9f9f9;">
              <p>Merhaba <strong>{toName}</strong>,</p>
              <p>Hesabınızı doğrulamak için aşağıdaki butona tıklayın:</p>
              <div style="text-align:center;margin:30px 0;">
                <a href="{confirmLink}" style="background:#27ae60;color:white;padding:14px 28px;text-decoration:none;border-radius:6px;font-weight:bold;">
                  E-postamı Doğrula
                </a>
              </div>
            </div>
            </body></html>
            """;

        await SendAsync(toEmail, toName, "E-posta Doğrulama", html);
    }
}
