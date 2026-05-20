namespace ECommerceApp.Helpers;

// Sayfalanmış liste yapısı - hem ürün, sipariş, yorum gibi listelerde
// hem de admin tablolarında kullanılır. Görünüm tarafında HasPreviousPage / HasNextPage ile
// önceki / sonraki butonları kontrol edilir.
public class PaginatedList<T>
{
    public List<T> Items { get; }       // Geçerli sayfadaki öğeler
    public int PageIndex { get; }       // 1-tabanlı sayfa numarası
    public int TotalPages { get; }      // Toplam sayfa sayısı
    public int TotalCount { get; }      // Filtre sonrası toplam kayıt sayısı
    public int PageSize { get; }        // Sayfa başına kayıt sayısı

    public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
    {
        Items = items;
        TotalCount = count;
        PageIndex = pageIndex;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
    }

    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;

    // IQueryable üzerinden çalışır - EF Core sorgusu Skip/Take ile DB tarafında sayfalanır
    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
    {
        var count = source.Count();
        var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        return new PaginatedList<T>(items, count, pageIndex, pageSize);
    }

    // Bellekteki bir koleksiyon için senkron sayfalama yardımcısı
    public static PaginatedList<T> Create(IEnumerable<T> source, int pageIndex, int pageSize)
    {
        var list = source.ToList();
        var count = list.Count;
        var items = list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        return new PaginatedList<T>(items, count, pageIndex, pageSize);
    }
}
