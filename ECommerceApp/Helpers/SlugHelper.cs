using System.Text.RegularExpressions;

namespace ECommerceApp.Helpers;

public static class SlugHelper
{
    public static string GenerateSlug(string text)
    {
        text = text.ToLowerInvariant();
        text = text
            .Replace('ş', 's').Replace('Ş', 's')
            .Replace('ı', 'i').Replace('İ', 'i')
            .Replace('ğ', 'g').Replace('Ğ', 'g')
            .Replace('ü', 'u').Replace('Ü', 'u')
            .Replace('ö', 'o').Replace('Ö', 'o')
            .Replace('ç', 'c').Replace('Ç', 'c');

        text = Regex.Replace(text, @"[^a-z0-9\s-]", "");
        text = Regex.Replace(text, @"\s+", "-");
        text = Regex.Replace(text, @"-+", "-");
        return text.Trim('-');
    }

    public static string MakeUnique(string slug, IEnumerable<string> existingSlugs)
    {
        if (!existingSlugs.Contains(slug)) return slug;

        var counter = 2;
        while (existingSlugs.Contains($"{slug}-{counter}"))
            counter++;

        return $"{slug}-{counter}";
    }
}
