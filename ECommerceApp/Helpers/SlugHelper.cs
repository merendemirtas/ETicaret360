using System.Text.RegularExpressions;

namespace ECommerceApp.Helpers;

// SEO-uyumlu URL parçaları (slug) üretmek için yardımcı sınıf.
// Türkçe karakterleri ASCII karşılıklarına çevirip, boşluk ve özel karakterleri tireye dönüştürür.
public static class SlugHelper
{
    // Verilen metni "ornek-urun-adi" formatında URL-dostu bir slug'a çevirir
    public static string GenerateSlug(string text)
    {
        text = text.ToLowerInvariant();

        // Türkçe karakter dönüşümleri (ş→s, ı→i, ğ→g, ü→u, ö→o, ç→c)
        text = text
            .Replace('ş', 's').Replace('Ş', 's')
            .Replace('ı', 'i').Replace('İ', 'i')
            .Replace('ğ', 'g').Replace('Ğ', 'g')
            .Replace('ü', 'u').Replace('Ü', 'u')
            .Replace('ö', 'o').Replace('Ö', 'o')
            .Replace('ç', 'c').Replace('Ç', 'c');

        // a-z, 0-9, boşluk ve tire dışındaki karakterleri kaldır
        text = Regex.Replace(text, @"[^a-z0-9\s-]", "");
        // Boşlukları tek tireye çevir
        text = Regex.Replace(text, @"\s+", "-");
        // Ardışık tireleri birleştir
        text = Regex.Replace(text, @"-+", "-");
        return text.Trim('-');
    }

    // Aynı slug daha önce kullanıldıysa "-2", "-3" gibi sayısal sonek ekleyerek benzersizleştirir
    public static string MakeUnique(string slug, IEnumerable<string> existingSlugs)
    {
        if (!existingSlugs.Contains(slug)) return slug;

        var counter = 2;
        while (existingSlugs.Contains($"{slug}-{counter}"))
            counter++;

        return $"{slug}-{counter}";
    }
}
